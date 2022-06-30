using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaSessionUtilities;

public class AESSecureMessagingWrapper : SecureMessagingWrapper
{
    private const string sscIVCipher = "AES/ECB/NoPadding";

    public AESSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, long ssc) : this(ksEnc, ksMac, 256, ssc) { }

    public AESSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, int maxTranceiveLength, long ssc)
    : base(ksEnc, ksMac, "AES/CBC/NoPadding", "AESCMAC")
    {
        //sscIVCipher = Util.getCipher("AES/ECB/NoPadding", Cipher.ENCRYPT_MODE, ksEnc);
    }

    /**
     * Returns the length (in bytes) to use for padding.
     * For AES this is 16.
     *
     * @return the length to use for padding
     */
    public override int getPadLength()
    {
        return 16;
    }

    public override byte[] CalculateMacForDo8eBlock(byte[] n)
        => Crypto.getAesCMac(KsMac, n);
    /**
     * Returns the send sequence counter as bytes, making sure
     * the 128 bit (16 byte) block-size is used.
     *
     * @return the send sequence counter as a 16 byte array
     */

    public override byte[] getEncodedSendSequenceCounter(long ssc) 
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

    //Was this not 
    private byte[] getIV()
    {
        throw new NotImplementedException();
        //return Crypto.getCipherText(sscIVCipher, ksEnc, new byte[0], getEncodedSendSequenceCounter());
    }

}
