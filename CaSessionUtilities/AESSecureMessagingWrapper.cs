using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaSessionUtilities;

public class AESSecureMessagingWrapper : SecureMessagingWrapper
{
    private byte[] ksEnc;
    private const string sscIVCipher = "AES/ECB/NoPadding";

  /**
   * Constructs a secure messaging wrapper based on the secure messaging
   * session keys and the initial value of the send sequence counter.
   * Used in BAC and EAC 1.
   *
   * @param ksEnc the session key for encryption
   * @param ksMac the session key for macs
   * @param ssc the initial value of the send sequence counter
   *
   * @throws GeneralSecurityException when the available JCE providers cannot provide the necessary cryptographic primitives
   */
  public AESSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, long ssc) : this(ksEnc, ksMac, 256, true, ssc) { }

    /**
     * Constructs a secure messaging wrapper based on the secure messaging
     * session keys and the initial value of the send sequence counter.
     * Used in BAC and EAC 1.
     *
     * @param ksEnc the session key for encryption
     * @param ksMac the session key for macs
     * @param maxTranceiveLength the maximum tranceive length, typical values are 256 or 65536
     * @param shouldCheckMAC a boolean indicating whether this wrapper will check the MAC in wrapped response APDUs
     * @param ssc the initial value of the send sequence counter
     *
     * @throws GeneralSecurityException when the available JCE providers cannot provide the necessary cryptographic primitives
     */
    public AESSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, int maxTranceiveLength, bool shouldCheckMAC, long ssc)
    :
        base(ksEnc, ksMac, "AES/CBC/NoPadding", "AESCMAC", maxTranceiveLength, shouldCheckMAC, ssc)
    {
        //sscIVCipher = Util.getCipher("AES/ECB/NoPadding", Cipher.ENCRYPT_MODE, ksEnc);
    }

    /**
     * Returns the length (in bytes) to use for padding.
     * For AES this is 16.
     *
     * @return the length to use for padding
     */
    protected override int getPadLength()
    {
        return 16;
    }

    /**
     * Returns the send sequence counter as bytes, making sure
     * the 128 bit (16 byte) block-size is used.
     *
     * @return the send sequence counter as a 16 byte array
     */

    protected override byte[] getEncodedSendSequenceCounter() 
        => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)getSendSequenceCounter() };


    //  public String toString()
    //{
    //    return new StringBuilder()
    //        .append("AESSecureMessagingWrapper [")
    //        .append("ssc: ").append(getSendSequenceCounter())
    //        .append(", kEnc: ").append(getEncryptionKey())
    //        .append(", kMac: ").append(getMACKey())
    //        .append(", shouldCheckMAC: ").append(shouldCheckMAC())
    //        .append(", maxTranceiveLength: ").append(getMaxTranceiveLength())
    //        .append("]")
    //        .toString();
    //}

    //  public int hashCode()
    //{
    //    return 71 * super.hashCode() + 17;
    //}

    //  public boolean equals(Object obj)
    //{
    //    if (this == obj)
    //    {
    //        return true;
    //    }
    //    if (obj == null)
    //    {
    //        return false;
    //    }
    //    if (getClass() != obj.getClass())
    //    {
    //        return false;
    //    }

    //    return super.equals(obj);
    //}

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

    protected override byte[] getIV()
    {
        return Crypto.getCipherText(sscIVCipher, ksEnc, getEncodedSendSequenceCounter());
    }
}
