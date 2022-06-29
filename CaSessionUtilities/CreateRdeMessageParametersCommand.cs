using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CaSessionUtilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace CaSessionUtilities;


/// <summary>
/// Should contain RDE 'Certificate'
/// TODO NB read all the DG14 oids before this point
/// </summary>
public record Args
{
    public CaSessionArgs CaSessionArgs { get; init; }
    public byte FileShortId { get; init; }
    public byte[] FileContent { get; init; }

    /// <summary>
    /// Max is 256 cos max transceive length
    /// </summary>
    public byte ReadLength { get; init; }
}


public record CaSessionArgs
{
    //caSessionArgs.getCaPublicKeyInfo().getKeyId(),
    //caSessionArgs.getCaPublicKeyInfo().getObjectIdentifier(),
    //caSessionArgs.getCaPublicKeyInfo().getSubjectPublicKey());
    //caSessionArgs.getCaInfo().getObjectIdentifier(),
    public string Oid { get; init; }
    public ChipAuthenticationPublicKeyInfo PublicKeyInfo { get; init; }
}


/// <summary>
/// All from DG14. This is the dto.
/// </summary>
public record ChipAuthenticationPublicKeyInfo
{
    public string Oid { get; init; }
    //EC or ECDH, ASN.1 DER?
    public byte[] PublicKey { get; init; }
    public BigInteger KeyId { get; init; }
}


public record RdeMessageParameters
{
    public byte[] EphemeralPublicKey { get; init; }
    public byte[] WrappedCommand { get; init; }
    public byte[] WrappedResponse { get; init; }
}

public class CreateRdeMessageParametersCommand
{
    public RdeMessageParameters Execute(Args args)
    {
        //DES or the AES wrapper chosen --> public static SecureMessagingWrapper EACCAProtocol.restartSecureMessaging
        var fakeCa = EACCAProtocol.doCA(args.CaSessionArgs.PublicKeyInfo.KeyId, args.CaSessionArgs.Oid, args.CaSessionArgs.PublicKeyInfo.Oid, args.CaSessionArgs.PublicKeyInfo.PublicKey);

        int shortFileId = args.FileShortId;
        var sfi = 0x80 | (shortFileId & 0xFF);
        var plainApdu = new CommandAPDU(ISO7816.CLA_ISO7816, ISO7816.INS_READ_BINARY, sfi, 0, args.ReadLength);
        var wrapped = fakeCa.Wrapper.wrap(plainApdu);
        //Trace.WriteLine("data      : " + wrapped.getData().PrettyHexFormat());
        var wrappedCommand = wrapped.ToArray();
        var responseEncoder = new AesSecureMessagingWrapperResponseEncoder(fakeCa.Wrapper.getEncryptionKey(), fakeCa.Wrapper.getMACKey());
        var wrappedResponse = responseEncoder.Write(Arrays.CopyOf(args.FileContent, args.ReadLength));

        return new RdeMessageParameters { EphemeralPublicKey = fakeCa.PcdPublicKey, WrappedCommand = wrappedCommand, WrappedResponse = wrappedResponse };
    }
}

public class EACCAProtocol
{

    /// <summary>
    /// We just want the correct wrapper
    /// This function create the correct flavour of DH key pair and passes the shared secret into the correct flavour of SecureMessageWrapper
    /// Problem here is the format of the piccPublicKeyDer which is read from the DG14. Java returns a PublicKey object.
    /// </summary>
    /// <param name="keyId"></param>
    /// <param name="oid"></param>
    /// <param name="publicKeyOID"></param>
    /// <param name="piccPublicKeyDer">DG14/param>
    /// <returns></returns>
    /// <exception cref="IllegalArgumentException"></exception>
    /// <exception cref="CardServiceException"></exception>
    public static RdeEACCAResult doCA(BigInteger keyId, string oid, string publicKeyOID, /*May not want to use this type cos it might not be portable*/ byte[] piccPublicKeyDer)
    {
        if (piccPublicKeyDer == null)
            throw new ArgumentException("PICC public key is null");

        var agreementAlg = ChipAuthenticationInfo.toKeyAgreementAlgorithm(oid);

        //TODO?
        //oid == oid ?? inferChipAuthenticationOIDfromPublicKeyOID(publicKeyOID);

        //..and pray it's an ASN1 object...
        var piccPublicKey = PublicKeyFactory.CreateKey(piccPublicKeyDer);
        var pcdEphemeralKeyPair = CreateKeyPair(agreementAlg, piccPublicKey);
        var sharedSecret = computeSharedSecret(agreementAlg, piccPublicKey, pcdEphemeralKeyPair.Private);
        var wrapper = restartSecureMessaging(oid, sharedSecret); //And we are done.

        return new RdeEACCAResult(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pcdEphemeralKeyPair.Public).GetDerEncoded(), wrapper);
    }

    private static AsymmetricCipherKeyPair CreateKeyPair(string agreementAlg, AsymmetricKeyParameter piccPublicKeyAsCipherParameter)
    {
        if ("DH".equals(agreementAlg))
        {
            var p = ((DHPublicKeyParameters) piccPublicKeyAsCipherParameter).Parameters;
            var keyPairGenerator = new DHKeyPairGenerator();
            keyPairGenerator.Init(new DHKeyGenerationParameters(new(), p));
            return keyPairGenerator.GenerateKeyPair();
        }
        
        if ("ECDH".equals(agreementAlg))
        {
            var p = ((ECPublicKeyParameters) piccPublicKeyAsCipherParameter).Parameters;
            var keyPairGenerator = new ECKeyPairGenerator();
            keyPairGenerator.Init(new ECKeyGenerationParameters(p, new()));
            return keyPairGenerator.GenerateKeyPair();
        }

        throw new InvalidOperationException();
    }

    public static byte[] computeSharedSecret(string alg, ICipherParameters piccPublicKey, ICipherParameters pcdPrivateKey)
    {
        var agreement = AgreementUtilities.GetBasicAgreement(alg);
        agreement.Init(pcdPrivateKey);
        return agreement.CalculateAgreement(piccPublicKey).ToByteArray();
    }

    public static SecureMessagingWrapper restartSecureMessaging(string oid, byte[] sharedSecret)
    {
        string cipherAlg = ChipAuthenticationInfo.toCipherAlgorithm(oid);
        int keyLength = ChipAuthenticationInfo.toKeyLength(oid);

        /* Start secure messaging. */
        byte[] ksEnc = Util.deriveKey(sharedSecret, cipherAlg, keyLength, Util.ENC_MODE);
        byte[] ksMac = Util.deriveKey(sharedSecret, cipherAlg, keyLength, Util.MAC_MODE);

        //if (cipherAlg.StartsWith("DESede"))
        //    return new DESedeSecureMessagingWrapper(ksEnc, ksMac, maxTranceiveLength, false, 0L);

        if (cipherAlg.StartsWith("AES"))
            return new AESSecureMessagingWrapper(ksEnc, ksMac, maxTranceiveLength, 0L);

        throw new InvalidOperationException("Unsupported cipher algorithm " + cipherAlg);
    }

    private const int maxTranceiveLength = 256;
}

public static class ChipAuthenticationInfo
{

    //  /** Used in Chip Authentication 1 and 2. */
    public const string ID_CA_DH_3DES_CBC_CBC = "0.4.0.127.0.7.2.2.2"; // EACObjectIdentifiers.id_CA_DH_3DES_CBC_CBC.getId();

    ///** Used in Chip Authentication 1 and 2. */
    public const string ID_CA_ECDH_3DES_CBC_CBC = "0.4.0.127.0.7.2.2.3.2.1 "; // EACObjectIdentifiers.id_CA_ECDH_3DES_CBC_CBC.getId();

    /** Used in Chip Authentication 1 and 2. */
    public const string ID_CA_DH_AES_CBC_CMAC_128 = "0.4.0.127.0.7.2.2.3.1.2";
    /** Used in Chip Authentication 1 and 2. */

    public const string ID_CA_DH_AES_CBC_CMAC_192 = "0.4.0.127.0.7.2.2.3.1.3";

    /** Used in Chip Authentication 1 and 2. */
    public const string ID_CA_DH_AES_CBC_CMAC_256 = "0.4.0.127.0.7.2.2.3.1.4";

    /** Used in Chip Authentication 1 and 2. */
    public const string ID_CA_ECDH_AES_CBC_CMAC_128 = "0.4.0.127.0.7.2.2.3.2.2";

    /** Used in Chip Authentication 1 and 2. */
    public const string ID_CA_ECDH_AES_CBC_CMAC_192 = "0.4.0.127.0.7.2.2.3.2.3";

    /** Used in Chip Authentication 1 and 2. */
    public const string ID_CA_ECDH_AES_CBC_CMAC_256 = "0.4.0.127.0.7.2.2.3.2.4";

    public static string toCipherAlgorithm(string oid)
    {
        if (ID_CA_DH_3DES_CBC_CBC.equals(oid)
            || ID_CA_ECDH_3DES_CBC_CBC.equals(oid))
        {
            return "DESede";
        }
        else if (ID_CA_DH_AES_CBC_CMAC_128.equals(oid)
            || ID_CA_DH_AES_CBC_CMAC_192.equals(oid)
            || ID_CA_DH_AES_CBC_CMAC_256.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_128.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_192.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_256.equals(oid))
        {
            return "AES";
        }

        throw new InvalidOperationException("Unsupported OID.");
    }

    public static int toKeyLength(string oid)
    {
        if (ID_CA_DH_3DES_CBC_CBC.equals(oid)
            || ID_CA_ECDH_3DES_CBC_CBC.equals(oid)
            || ID_CA_DH_AES_CBC_CMAC_128.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_128.equals(oid))
        {
            return 128;
        }
        else if (ID_CA_DH_AES_CBC_CMAC_192.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_192.equals(oid))
        {
            return 192;
        }
        else if (ID_CA_DH_AES_CBC_CMAC_256.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_256.equals(oid))
        {
            return 256;
        }

        throw new InvalidOperationException("Unsupported OID.");
    }

    /// <summary>
    /// OID of DG14 CA Session Public Key
    /// </summary>
    /// <param name="oid"></param>
    /// <returns></returns>
    /// <exception cref="NumberFormatException"></exception>
    public static string toKeyAgreementAlgorithm(string oid)
    {
        if (oid == null)
            throw new ArgumentException("Unknown OID: null");
        

        if (ID_CA_DH_3DES_CBC_CBC.equals(oid)
            || ID_CA_DH_AES_CBC_CMAC_128.equals(oid)
            || ID_CA_DH_AES_CBC_CMAC_192.equals(oid)
            || ID_CA_DH_AES_CBC_CMAC_256.equals(oid))
        {
            return "DH";
        }
        else if (ID_CA_ECDH_3DES_CBC_CBC.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_128.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_192.equals(oid)
            || ID_CA_ECDH_AES_CBC_CMAC_256.equals(oid))
        {
            return "ECDH";
        }

        throw new InvalidOperationException("Unsupported OID.");
    }
}

public record RdeEACCAResult
{
    public byte[] PcdPublicKey { get; }
    public SecureMessagingWrapper Wrapper { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pcdPublicKey">Something that can be encoded that Java understands</param>
    /// <param name="wrapper"></param>
    public RdeEACCAResult(byte[] pcdPublicKey, SecureMessagingWrapper wrapper)
    {
        PcdPublicKey = pcdPublicKey;
        Wrapper = wrapper;
    }
}