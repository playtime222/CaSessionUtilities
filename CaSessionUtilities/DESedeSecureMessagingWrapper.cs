//using CaSessionUtilities;

///**
// * Secure messaging wrapper for APDUs.
// * Initially based on Section E.3 of ICAO-TR-PKI.
// *
// * @author The JMRTD team (info@jmrtd.org)
// *
// * @version $Revision: 1805 $
// */
//public class DESedeSecureMessagingWrapper : SecureMessagingWrapper
//{
//    /** Initialization vector consisting of 8 zero bytes. */
//    public static readonly byte[] ZERO_IV_PARAM_SPEC = { 0, 0, 0, 0, 0, 0, 0, 0 };

//    /**
//     * Constructs a secure messaging wrapper based on the secure messaging
//     * session keys. The initial value of the send sequence counter is set to
//     * <code>0L</code>.
//     *
//     * @param ksEnc the session key for encryption
//     * @param ksMac the session key for macs
//     *
//     * @throws GeneralSecurityException
//     *             when the available JCE providers cannot provide the necessary
//     *             cryptographic primitives
//     *             ({@code "DESede/CBC/Nopadding"} Cipher, {@code "ISO9797Alg3Mac"} Mac).
//     */
//    public DESedeSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac)
//    :
//        this(ksEnc, ksMac, true)
//    {
//    }

//    /**
//     * Constructs a secure messaging wrapper based on the secure messaging
//     * session keys. The initial value of the send sequence counter is set to
//     * {@code 0L}.
//     *
//     * @param ksEnc the session key for encryption
//     * @param ksMac the session key for macs
//     * @param shouldCheckMAC a boolean indicating whether this wrapper will check the MAC in wrapped response APDUs
//     *
//     * @throws GeneralSecurityException
//     *             when the available JCE providers cannot provide the necessary
//     *             cryptographic primitives
//     *             ({@code "DESede/CBC/Nopadding"} Cipher, {@code "ISO9797Alg3Mac"} Mac).
//     */
//    public DESedeSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, bool shouldCheckMAC)
//    :
//        this(ksEnc, ksMac, 256, shouldCheckMAC, 0L)
//    {
//    }

//    /**
//     * Constructs a secure messaging wrapper based on the secure messaging
//     * session keys and the initial value of the send sequence counter.
//     * Used in BAC and EAC 1.
//     *
//     * @param ksEnc the session key for encryption
//     * @param ksMac the session key for macs
//     * @param ssc the initial value of the send sequence counter
//     *
//     * @throws GeneralSecurityException when the available JCE providers cannot provide the necessary cryptographic primitives
//     */
//    public DESedeSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, long ssc)
//    :
//    this(ksEnc, ksMac, 256, true, ssc)
//    {
//    }


//    /**
//     * Constructs a secure messaging wrapper based on the secure messaging
//     * session keys and the initial value of the send sequence counter.
//     * Used in BAC and EAC 1.
//     *
//     * @param ksEnc the session key for encryption
//     * @param ksMac the session key for macs
//     * @param maxTranceiveLength the maximum tranceive length, typical values are 256 or 65536
//     * @param shouldCheckMAC a boolean indicating whether this wrapper will check the MAC in wrapped response APDUs
//     * @param ssc the initial value of the send sequence counter
//     *
//     * @throws GeneralSecurityException when the available JCE providers cannot provide the necessary cryptographic primitives
//     */
//    public DESedeSecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, int maxTranceiveLength, bool shouldCheckMAC, long ssc)
//        : base(ksEnc, ksMac,
//            //"DESede/CBC/NoPadding", "ISO9797Alg3Mac",
//            maxTranceiveLength
//            //, ssc
//            )
//    {
//    }

//    /**
//     * Returns the type of secure messaging wrapper.
//     * In this case {@code "DESede"} will be returned.
//     *
//     * @return the type of secure messaging wrapper
//     */
//    public String getType()
//    {
//        return "DESede";
//    }

//    /**
//     * Returns the length (in bytes) to use for padding.
//     * For 3DES this is 8.
//     *
//     * @return the length to use for padding
//     */

//    public override int getPadLength()
//    {
//        return 8;
//    }

//    public override byte[] getEncodedSendSequenceCounter(long ssc)
//    {
//        var byteArrayOutputStream = new MemoryStream();
//        var dataOutputStream = new BinaryWriter(byteArrayOutputStream);
//        dataOutputStream.Write(ssc); //TODO Check this one is the correct way around
//        byteArrayOutputStream.Dispose();
//        return byteArrayOutputStream.ToArray();
//    }

//    public override byte[] Encrypt(byte[] plainText, byte[] encodedSsc) =>
//        Crypto.getDESedeCBCNoPaddingCipherText(ksEnc, getIV(null), plainText);

//    public override byte[] CalculateMac(byte[] content)
//        => Crypto.getAesCMac(ksEnc, content);

//    public override byte[] getIV(byte[] _)
//    {
//        return ZERO_IV_PARAM_SPEC;
//    }
//}
