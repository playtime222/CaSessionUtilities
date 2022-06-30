using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace CaSessionUtilities;

public class AesSecureMessagingWrapper : SecureMessagingWrapper
{
    private const string sscIVCipherName = "AES/ECB/NoPadding";

    public AesSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, long ssc) : this(ksEnc, ksMac) { }

    public AesSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac)
    : base(ksEnc, ksMac, "AES/CBC/NoPadding", "AESCMAC")
    {
        //sscIVCipher = Util.getCipher("AES/ECB/NoPadding", Cipher.ENCRYPT_MODE, ksEnc);
    }

    //private byte[] iv;

    private byte[] getIv(byte[] ksEnc, byte[] encodedSSC)
    {
        var sscIVCipher = CipherUtilities.GetCipher(sscIVCipherName);
        sscIVCipher.Init(true, new KeyParameter(ksEnc));
        return sscIVCipher.DoFinal(encodedSSC);
    }

    /**
     * Returns the length (in bytes) to use for padding.
     * For AES this is 16.
     *
     * @return the length to use for padding
     */
    public override int BlockSize => 16;

    public override byte[] CalculateMac(byte[] data)
        => Crypto.getAesCMac(KsMac, data);

    public override byte[] GetEncodedDataForResponse(byte[] response)
    {
        var iv = getIv(KsEnc, GetEncodedSendSequenceCounter(2)); //Contains state -> 2 the SSC counter...
        return Crypto.getAESCBCNoPaddingCipherText(KsEnc, iv, response);
    }

    /**
     * Returns the send sequence counter as bytes, making sure
     * the 128 bit (16 byte) block-size is used.
     *
     * @return the send sequence counter as a 16 byte array
     */

    public override byte[] GetEncodedSendSequenceCounter(long ssc) 
        => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)ssc };

    /**
     * Returns the IV by encrypting the send sequence counter.
     *
     * AES uses IV = E K_Enc , SSC), see ICAO SAC TR Section 4.6.3.
     *
     * @return the initialization vector specification
     *
     * @throws GeneralSecurityException on error
     */

    //protected override IvParameterSpec getIV()
    //{
    //    byte[] encryptedSSC = sscIVCipher.doFinal(getEncodedSendSequenceCounter());
    //    return new IvParameterSpec(encryptedSSC);
    //}
}
