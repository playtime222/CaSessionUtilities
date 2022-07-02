using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace CaSessionUtilities.Wrapping.Implementation;

public class EACCAProtocol
{

    /// <summary>
    /// Cut down the result to the correct wrapper and the session ephemeral public key Z
    /// This function create the correct flavour of DH key pair and passes the shared secret into the correct flavour of SecureMessageWrapper
    /// Problem here is the format of the piccPublicKeyDer which is read from the DG14. Java returns a PublicKey object.
    /// </summary>
    /// <param name="keyId"></param>
    /// <param name="oid"></param>
    /// <param name="publicKeyOID"></param>
    /// <param name="piccPublicKeyDer">DG14/param>
    /// <returns></returns>
    public static RdeEACCAResult doCA(BigInteger keyId, string oid, string publicKeyOID, /*May not want to use this type cos it might not be portable*/ byte[] piccPublicKeyDer)
    {
        if (piccPublicKeyDer == null)
            throw new ArgumentException("PICC public key is null");

        //TODO and the rest...

        var agreementAlg = ChipAuthenticationInfo.toKeyAgreementAlgorithm(oid);

        //TODO?
        //oid == oid ?? inferChipAuthenticationOIDfromPublicKeyOID(publicKeyOID);

        //..and pray it's an ASN1 object...
        var piccPublicKey = PublicKeyFactory.CreateKey(piccPublicKeyDer);
        var pcdEphemeralKeyPair = CreateKeyPair(agreementAlg, piccPublicKey);
        var sharedSecret = ComputeSharedSecret(agreementAlg, piccPublicKey, pcdEphemeralKeyPair.Private);
        var wrapper = RestartSecureMessaging(oid, sharedSecret); //And we are done.

        return new RdeEACCAResult(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pcdEphemeralKeyPair.Public).GetDerEncoded(), wrapper);
    }

    private static AsymmetricCipherKeyPair CreateKeyPair(string agreementAlg, AsymmetricKeyParameter piccPublicKeyAsCipherParameter)
    {
        if ("DH".Equals(agreementAlg, StringComparison.InvariantCultureIgnoreCase))
        {
            var p = ((DHPublicKeyParameters)piccPublicKeyAsCipherParameter).Parameters;
            var keyPairGenerator = new DHKeyPairGenerator();
            keyPairGenerator.Init(new DHKeyGenerationParameters(new(), p));
            return keyPairGenerator.GenerateKeyPair();
        }

        if ("ECDH".Equals(agreementAlg, StringComparison.InvariantCultureIgnoreCase))
        {
            var p = ((ECPublicKeyParameters)piccPublicKeyAsCipherParameter).Parameters;
            var keyPairGenerator = new ECKeyPairGenerator();
            keyPairGenerator.Init(new ECKeyGenerationParameters(p, new()));
            return keyPairGenerator.GenerateKeyPair();
        }

        throw new InvalidOperationException("Unsupported agreement algorithm.");
    }

    private static byte[] ComputeSharedSecret(string alg, ICipherParameters piccPublicKey, ICipherParameters pcdPrivateKey)
    {
        var agreement = AgreementUtilities.GetBasicAgreement(alg);
        agreement.Init(pcdPrivateKey);
        return agreement.CalculateAgreement(piccPublicKey).ToByteArray();
    }

    private static SecureMessagingWrapper RestartSecureMessaging(string oid, byte[] sharedSecret)
    {
        var cipherInfo = ChipAuthenticationInfo.Find(oid);

        /* Start secure messaging. */
        var ksEnc = SessionMessagingWrapperKeyUtility.DeriveKey(sharedSecret, cipherInfo, SessionMessagingWrapperKeyUtility.ENC_MODE);
        var ksMac = SessionMessagingWrapperKeyUtility.DeriveKey(sharedSecret, cipherInfo, SessionMessagingWrapperKeyUtility.MAC_MODE);

        if (cipherInfo.Algorithm.StartsWith("DESede"))
            return new DESedeSecureMessagingWrapper(ksEnc, ksMac);

        if (cipherInfo.Algorithm.StartsWith("AES"))
            return new AesSecureMessagingWrapper(ksEnc, ksMac);

        throw new InvalidOperationException("Unsupported cipher algorithm.");
    }
}