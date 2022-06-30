using System.Diagnostics;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;



/// <summary>
/// JUST the encryption methods required during the wrapping of the command and the response
/// </summary>
public abstract class SecureMessagingWrapper
{
    public string CipherAlg { get; }
    public string MacAlg { get; }

    //private readonly string cipherAlg;
    public byte[] KsEnc { get; }
    public byte[] KsMac { get; }
    public int MaxTranceiveLength = 256;

    /**
     * Constructs a secure messaging wrapper based on the secure messaging
     * session keys and the initial value of the send sequence counter.
     *
     * @param ksEnc the session key for encryption
     * @param ksMac the session key for message authenticity
     * @param cipherAlg the mnemonic Java string describing the cipher algorithm
     * @param macAlg the mnemonic Java string describing the message authenticity checking algorithm
     * @param maxTranceiveLength the maximum tranceive length, typical values are 256 or 65536
     * @param shouldCheckMAC a bool indicating whether this wrapper will check the MAC in wrapped response APDUs
     * @param ssc the initial value of the send sequence counter
     *
     * @throws GeneralSecurityException when the available JCE providers cannot provide the necessary cryptographic primitives
     */
    protected SecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, string cipherAlg, string macAlg)
    {
        CipherAlg = cipherAlg;
        MacAlg = macAlg;

        KsEnc = ksEnc;
        KsMac = ksMac;

        //this.cipher = Util.getCipher(cipherAlg);
        //this.mac = Util.getAesCMac(macAlg);
    }

    /**
     * Returns a copy of the given wrapper, with an identical (but perhaps independent)
     * state for known secure messaging wrapper types. If the wrapper type is not recognized
     * the original wrapper is returned.
     *
     * @param wrapper the original wrapper
     *
     * @return a copy of that wrapper
     */
    //public static SecureMessagingWrapper getInstance(SecureMessagingWrapper wrapper)
    //{
    //    try
    //    {
    //        if (wrapper is DESedeSecureMessagingWrapper) {
    //            DESedeSecureMessagingWrapper desEDESecureMessagingWrapper = (DESedeSecureMessagingWrapper)wrapper;
    //            return new DESedeSecureMessagingWrapper(desEDESecureMessagingWrapper);
    //        } else if (wrapper instanceof AESSecureMessagingWrapper) {
    //            AESSecureMessagingWrapper aesSecureMessagingWrapper = (AESSecureMessagingWrapper)wrapper;
    //            return new AESSecureMessagingWrapper(aesSecureMessagingWrapper);
    //        }
    //    }
    //    catch (GeneralSecurityException gse)
    //    {
    //        LOGGER.log(Level.WARNING, "Could not copy wrapper", gse);
    //    }

    //    LOGGER.warning("Not copying wrapper");
    //    return wrapper;
    //}

    /**
     * Returns the current value of the send sequence counter.
     *
     * @return the current value of the send sequence counter.
     */
    //public long getSendSequenceCounter()
    //{
    //    return ssc;
    //}
    public abstract byte[] getEncodedSendSequenceCounter(long ssc);
    /**
     * Returns the shared key for encrypting APDU payloads.
     *
     * @return the encryption key
     */
    public byte[] getEncryptionKey()
    {
        return KsEnc;
    }

    /**
     * Returns the shared key for computing message authentication codes over APDU payloads.
     *
     * @return the MAC key
     */
    public byte[] getMACKey()
    {
        return KsMac;
    }

    /**
     * Wraps the APDU buffer of a command APDU.
     * As a side effect, this method increments the internal send
     * sequence counter maintained by this wrapper.
     *
     * @param commandAPDU buffer containing the command APDU
     *
     * @return length of the command APDU after wrapping
     */


    /**
     * Returns the length (in bytes) to use for padding.
     * @return the length to use for padding
     */
    public abstract int getPadLength();

    /**
     * Returns the initialization vector to be used by the encryption cipher.
     *
     * @return the initialization vector as a paramaters specification
     *
     * @throws GeneralSecurityException on error constructing the parameter specification object
     */
    //protected abstract byte[] getIV();

    /**
     * Returns the send sequence counter encoded as a byte array for inclusion in wrapped APDUs.
     *
     * @return the send sequence counter encoded as byte array
     */
    public abstract byte[] CalculateMacForDo8eBlock(byte[] n);

    /* PRIVATE BELOW. */

    /*
     * The SM Data Objects (see [ISO/IEC 7816-4]) MUST be used in the following order:
     *   - Command APDU: [DO‘85’ or DO‘87’] [DO‘97’] DO‘8E’.
     *   - Response APDU: [DO‘85’ or DO‘87’] [DO‘99’] DO‘8E’.
     */



}



public class CommandEncoder
{

    private const long SSC = 1;

    private SecureMessagingWrapper _Wrapper;

    public CommandEncoder(SecureMessagingWrapper wrapper)
    {
        _Wrapper = wrapper;
    }

    public CommandAPDU wrap(CommandAPDU commandAPDU)
    {
        return wrapCommandAPDU(commandAPDU);
    }
    
    /**
     * Performs the actual encoding of a command APDU.
     * Based on Section E.3 of ICAO-TR-PKI, especially the examples.
     *
     * @param commandAPDU the command APDU
     *
     * @return a byte array containing the wrapped APDU buffer
     */
    private CommandAPDU wrapCommandAPDU(CommandAPDU commandAPDU)
    {
        int cla = commandAPDU.CLA;
        int ins = commandAPDU.INS;
        int p1 = commandAPDU.P1;
        int p2 = commandAPDU.P2;
        //int lc = commandAPDU.Nc;
        int le = commandAPDU.Ne;

        byte[] maskedHeader = { (byte)(cla | (byte)0x0C), (byte)ins, (byte)p1, (byte)p2 };
        byte[] paddedMaskedHeader = Util.pad(maskedHeader, _Wrapper.getPadLength());

        bool hasDO85 = ((byte)commandAPDU.INS == ISO7816.INS_READ_BINARY2);

        byte[] do8587 = new byte[0];
        byte[] do97 = new byte[0];


        /* Include the expected length, if present. */
        if (le > 0)
        {
            do97 = TLVUtil.wrapDO(0x97, encodeLe(le));
        }

        /* Encrypt command data, if present. */
        //if (lc > 0)
        //{
        //    byte[] paddedCommandApdu = Util.pad(commandAPDU.getData(), getPadLength());
        //    /* Re-initialize cipher, this time with IV based on SSC. */
        //    //cipher.init(Cipher.ENCRYPT_MODE, ksEnc, getIV());
        //    byte[] ciphertext = Crypto.getCipherText(cipherAlg, ksEnc, getIV(), paddedCommandApdu);

        //    memoryStream.Position = 0;
        //    memoryStream.write(hasDO85 ? (byte)0x85 : (byte)0x87);
        //    memoryStream.write(TLVUtil.getLengthAsBytes(ciphertext.Length + (hasDO85 ? 0 : 1)));
        //    if (!hasDO85)
        //    {
        //        memoryStream.write(0x01);
        //    }
        //    memoryStream.write(ciphertext);
        //    do8587 = memoryStream.ToArray();
        //}

        var n = getN(paddedMaskedHeader, do8587, do97);
        var do8E = GetDo8eBlock(n);

        /* Construct protected APDU... */
        using var memoryStream = new MemoryStream();
        memoryStream.Write(do8587);
        memoryStream.Write(do97);
        memoryStream.Write(do8E);
        var data = memoryStream.ToArray();

        /*
         * The requested response is 0x00 or 0x0000, depending on whether extended length is needed.
         */
        if (le <= 256 && data.Length <= 255)
        {
            return new CommandAPDU(maskedHeader[0], maskedHeader[1], maskedHeader[2], maskedHeader[3], data, 256);
        }
        else if (le > 256 || data.Length > 255)
        {
            return new CommandAPDU(maskedHeader[0], maskedHeader[1], maskedHeader[2], maskedHeader[3], data, 65536);
        }
        else
        {
            /* Not sure if this case ever occurs, but this is consistent with previous behavior. */
            return new CommandAPDU(maskedHeader[0], maskedHeader[1], maskedHeader[2], maskedHeader[3], data, _Wrapper.MaxTranceiveLength);
        }
    }

    private byte[] GetDo8eBlock(byte[] n)
    {
        //var cc = Crypto.getAesCMac(/*macAlg,*/ _KsMac, n);
        var cc = _Wrapper.CalculateMacForDo8eBlock(n);


        Trace.WriteLine($"{"mac",-10}: {cc.PrettyHexFormat()}");
        const int ccLength = 8;
        using MemoryStream memoryStream = new();
        memoryStream.write((byte)0x8E);
        memoryStream.write(ccLength); //
        memoryStream.write(cc, 0, ccLength);
        var do8E = memoryStream.ToArray();
        Trace.WriteLine($"{"d08e",-10}: {do8E.PrettyHexFormat()}");
        return do8E;
    }

    private byte[] getN(byte[] paddedMaskedHeader, byte[] do8587, byte[] do97)
    {
        using var ms = new MemoryStream();

        var encSeq = _Wrapper.getEncodedSendSequenceCounter(SSC);
        Trace.WriteLine($"{"ssc",-10}: {encSeq.PrettyHexFormat()}");

        ms.write(encSeq);
        ms.write(paddedMaskedHeader);
        ms.write(do8587);
        ms.write(do97);
        var n = Util.pad(ms.ToArray(), _Wrapper.getPadLength());
        Trace.WriteLine($"{"n",-10}: {n.PrettyHexFormat()}");
        return n;
    }

    /**
     * Encodes the expected length value to a byte array for inclusion in wrapped APDUs.
     * The result is a byte array of length 1 or 2.
     *
     * @param le a non-negative expected length
     *
     * @return a byte array with the encoded expected length
     */
    public static byte[] encodeLe(int le)
    {
        if (0 <= le && le <= 256)
        {
            /* NOTE: Both 0x00 and 0x100 are mapped to 0x00. */
            return new byte[] { (byte)le };
        }
        else
        {
            return new byte[] { (byte)((le & 0xFF00) >> 8), (byte)(le & 0xFF) };
        }
    }

}