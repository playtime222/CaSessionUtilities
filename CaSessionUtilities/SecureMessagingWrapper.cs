using System.Diagnostics;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

/**
     * Secure messaging wrapper base class.
     * @author The JMRTD team
     *
     * @version $Revision: 1807 $
     */
public abstract class SecureMessagingWrapper
{

    private readonly int maxTranceiveLength;
    private readonly bool _shouldCheckMAC;
    private long ssc;


    //private readonly string cipherAlg;
    private readonly string macAlg;
    private readonly byte[] ksEnc;
    private readonly byte[] ksMac;

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
    protected SecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, string cipherAlg, string macAlg, int maxTranceiveLength, bool shouldCheckMAC, long ssc)
    {
        this.maxTranceiveLength = maxTranceiveLength;
        _shouldCheckMAC = shouldCheckMAC;

        this.macAlg = macAlg;
        //this.cipherAlg= cipherAlg;

        this.ksEnc = ksEnc;
        this.ksMac = ksMac;
        this.ssc = ssc;

        //this.cipher = Util.getCipher(cipherAlg);
        //this.mac = Util.getMac(macAlg);
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
    public long getSendSequenceCounter()
    {
        return ssc;
    }

    /**
     * Returns the shared key for encrypting APDU payloads.
     *
     * @return the encryption key
     */
    public byte[] getEncryptionKey()
    {
        return ksEnc;
    }

    /**
     * Returns the shared key for computing message authentication codes over APDU payloads.
     *
     * @return the MAC key
     */
    public byte[] getMACKey()
    {
        return ksMac;
    }

    /**
     * Returns a bool indicating whether this wrapper will check the MAC in wrapped response APDUs.
     *
     * @return a bool indicating whether this wrapper will check the MAC in wrapped response APDUs
     */
    public bool shouldCheckMAC()
    {
        return _shouldCheckMAC;
    }

    /**
     * Returns the maximum tranceive length of wrapped command and response APDUs,
     * typical values are 256 and 65536.
     *
     * @return the maximum tranceive length of wrapped command and response APDUs
     */
    public int getMaxTranceiveLength()
    {
        return maxTranceiveLength;
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
    public CommandAPDU wrap(CommandAPDU commandAPDU)
    {
        ssc++;
        return wrapCommandAPDU(commandAPDU);
    }

    /**
     * Unwraps the APDU buffer of a response APDU.
     *
     * @param responseAPDU the response APDU
     *
     * @return a new byte array containing the unwrapped buffer
     */
    //public ResponseAPDU unwrap(ResponseAPDU responseAPDU)
    //{
    //    ssc++;
    //    try
    //    {
    //        byte[] data = responseAPDU.getData();
    //        if (data == null || data.Length <= 0)
    //        {
    //            // no sense in unwrapping - card indicates some kind of error
    //            throw new InvalidOperationException("Card indicates SM error, SW = " + Integer.toHexString(responseAPDU.getSW() & 0xFFFF));
    //            /* FIXME: wouldn't it be cleaner to throw a CardServiceException? */
    //        }
    //        return unwrapResponseAPDU(responseAPDU);
    //    }
    //    catch (GeneralSecurityException gse)
    //    {
    //        throw new InvalidOperationException("Unexpected exception", gse);
    //    }
    //    catch (IOException ioe)
    //    {
    //        throw new InvalidOperationException("Unexpected exception", ioe);
    //    }
    //}

    /**
     * Checks the MAC.
     *
     * @param rapdu the bytes of the response APDU, including the {@code 0x8E} tag, the length of the MAC, the MAC itself, and the status word
     * @param cc the MAC sent by the other party
     *
     * @return whether the computed MAC is identical
     *
     * @throws GeneralSecurityException on security related error
     */
    //protected bool checkMac(byte[] rapdu, byte[] cc)
    //{
    //    MemoryStream memoryStream = new MemoryStream();
    //        //DataOutputStream dataOutputStream = new DataOutputStream(MemoryStream);
    //    var dataOutputStream = memoryStream;
    //    dataOutputStream.Write(getEncodedSendSequenceCounter());
    //    byte[] paddedData = Util.pad(rapdu, 0, rapdu.Length - 2 - 8 - 2, getPadLength());
    //    dataOutputStream.Write(paddedData, 0, paddedData.Length);
    //    dataOutputStream.Flush();
    //    dataOutputStream.Close();
    //    //mac.init(ksMac);
    //    byte[] cc2 = Crypto.getMac(macAlg, ksMac, dataOutputStream.ToArray());

    //    if (cc2.Length > 8 && cc.Length == 8)
    //    {
    //        byte[] newCC2 = new byte[8];
    //        Array.Copy(cc2, 0, newCC2, 0, newCC2.Length);
    //        cc2 = newCC2;
    //    }

    //    return Arrays.Equals(cc, cc2);
    //}

    /**
     * Returns the length (in bytes) to use for padding.
     *
     * @return the length to use for padding
     */
    protected abstract int getPadLength();

    /**
     * Returns the initialization vector to be used by the encryption cipher.
     *
     * @return the initialization vector as a paramaters specification
     *
     * @throws GeneralSecurityException on error constructing the parameter specification object
     */
    protected abstract byte[] getIV();

    /**
     * Returns the send sequence counter encoded as a byte array for inclusion in wrapped APDUs.
     *
     * @return the send sequence counter encoded as byte array
     */
    protected abstract byte[] getEncodedSendSequenceCounter();

    /* PRIVATE BELOW. */

    /*
     * The SM Data Objects (see [ISO/IEC 7816-4]) MUST be used in the following order:
     *   - Command APDU: [DO‘85’ or DO‘87’] [DO‘97’] DO‘8E’.
     *   - Response APDU: [DO‘85’ or DO‘87’] [DO‘99’] DO‘8E’.
     */

    /**
     * Performs the actual encoding of a command APDU.
     * Based on Section E.3 of ICAO-TR-PKI, especially the examples.
     *
     * @param commandAPDU the command APDU
     *
     * @return a byte array containing the wrapped APDU buffer
     *
     * @throws GeneralSecurityException on error wrapping the APDU
     * @throws IOException on error writing the result to memory
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
        byte[] paddedMaskedHeader = Util.pad(maskedHeader, getPadLength());

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
            return new CommandAPDU(maskedHeader[0], maskedHeader[1], maskedHeader[2], maskedHeader[3], data, getMaxTranceiveLength());
        }
    }

    private byte[] GetDo8eBlock(byte[] n)
    {
        var cc = Crypto.getMac(macAlg, ksMac, n);
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

        var encSeq = getEncodedSendSequenceCounter();
        Trace.WriteLine($"{"ssc",-10}: {encSeq.PrettyHexFormat()}");

        ms.write(encSeq);
        ms.write(paddedMaskedHeader);
        ms.write(do8587);
        ms.write(do97);
        var n = Util.pad(ms.ToArray(), getPadLength());
        Trace.WriteLine($"{"n",-10}: {n.PrettyHexFormat()}");
        return n;
    }

    /**
       * Unwraps a response APDU sent by the ICC.
       * Based on Section E.3 of TR-PKI, especially the examples.
       *
       * @param responseAPDU the response APDU
       *
       * @return a byte array containing the unwrapped APDU buffer
       *
       * @throws GeneralSecurityException on error unwrapping the APDU
       * @throws IOException on error writing the result to memory
       */
    //  private ResponseAPDU unwrapResponseAPDU(ResponseAPDU responseAPDU) 
    //  {
    //    byte[] rapdu = responseAPDU.getBytes();
    //if (rapdu == null || rapdu.Length < 2)
    //{
    //    throw new ArgumentException("Invalid response APDU");
    //}
    //cipher.init(Cipher.DECRYPT_MODE, ksEnc, getIV());

    //byte[] data = new byte[0];
    //byte[] cc = null;
    //short sw = 0;
    //var memoryStream = new MemoryStream(rapdu);
    //BinaryReader inputStream = new BinaryReader(memoryStream);
    //try
    //{
    //    bool isFinished = false;
    //    while (!isFinished)
    //    {
    //        int tag = memoryStream.ReadByte();
    //        switch (tag)
    //        {
    //            case (byte)0x87:
    //                data = readDO87(inputStream, false);
    //                break;
    //            case (byte)0x85:
    //                data = readDO87(inputStream, true);
    //                break;
    //            case (byte)0x99:
    //                sw = readDO99(inputStream);
    //                break;
    //            case (byte)0x8E:
    //                cc = readDO8E(inputStream);
    //                isFinished = true;
    //                break;
    //            default:
    //                        throw new Exception();
    //                break;
    //        }
    //    }
    //}
    //finally
    //{
    //    inputStream.close();
    //}
    //if (shouldCheckMAC() && !checkMac(rapdu, cc))
    //{
    //    throw new InvalidOperationException("Invalid MAC");
    //}
    //MemoryStream bOut = new MemoryStream();
    //bOut.write(data, 0, data.Length);
    //bOut.write((sw & 0xFF00) >> 8);
    //bOut.write(sw & 0x00FF);
    //return new ResponseAPDU(bOut.ToArray());
    //  }

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

    /**
     * Reads a data object.
     * The {@code 0x87} tag has already been read.
     *
     * @param inputStream the stream to read from
     * @param do85 whether to expect a {@code 0x85} (including an extra 1 length) data object.
     *
     * @return the bytes that were read
     *
     * @throws IOException on error reading from the stream
     * @throws GeneralSecurityException on error decrypting the data
     */
    //private byte[] readDO87(BinaryReader inputStream, bool do85) {

    //    /* Read length... */
    //    int length = 0;
    //int buf = inputStream.readUnsignedByte();
    //if ((buf & 0x00000080) != 0x00000080)
    //{
    //    /* Short form */
    //    length = buf;
    //    if (!do85)
    //    {
    //        buf = inputStream.readUnsignedByte(); /* should be 0x01... */
    //        if (buf != 0x01)
    //        {
    //            throw new InvalidOperationException("DO'87 expected 0x01 marker, found " + Hex.ToHexString(new byte[] { (byte)(buf & 0xFF) }));
    //        }
    //    }
    //}
    //else
    //{
    //    /* Long form */
    //    int lengthBytesCount = buf & 0x0000007F;
    //    for (int i = 0; i < lengthBytesCount; i++)
    //    {
    //        length = (length << 8) | inputStream.readUnsignedByte();
    //    }
    //    if (!do85)
    //    {
    //        buf = inputStream.readUnsignedByte(); /* should be 0x01... */
    //        if (buf != 0x01)
    //        {
    //            throw new InvalidOperationException("DO'87 expected 0x01 marker");
    //        }
    //    }
    //}
    //if (!do85)
    //{
    //    length--; /* takes care of the extra 0x01 marker... */
    //}
    //    /* Read, decrypt, unpad the data... */
    //    byte[] ciphertext = new byte[length];
    //    inputStream.readFully(ciphertext);
    //    byte[] paddedData = cipher.doFinal(ciphertext);
    //    return Util.unpad(paddedData);
    //  }

    /**
     * Reads a data object.
     * The {@code 0x99} tag has already been read.
     *
     * @param inputStream the stream to read from
     *
     * @return the status word
     *
     * @throws IOException on error reading from the stream
     */
    //  private short readDO99(BinaryReader inputStream) 
    //{
    //    int length = inputStream.readUnsignedByte();
    //    if (length != 2) {
    //        throw new InvalidOperationException("DO'99 wrong length");
    //    }
    //    byte sw1 = inputStream.readByte();
    //    byte sw2 = inputStream.readByte();
    //    return (short)(((sw1 & 0x000000FF) << 8) | (sw2 & 0x000000FF));
    //}

    /**
     * Reads a data object.
     * This assumes that the {@code 0x8E} tag has already been read.
     *
     * @param inputStream the stream to read from
     *
     * @return the bytes that were read
     *
     * @throws IOException on error
     */
    //private byte[] readDO8E(BinaryReader inputStream) 
    //{
    //    int length = inputStream.readUnsignedByte();
    //    if (length != 8 && length != 16) {
    //        throw new InvalidOperationException("DO'8E wrong length for MAC: " + length);
    //    }
    //    byte[]
    //    cc = new byte[length];
    //inputStream.readFully(cc);
    //return cc;
    //  }

    //  @Override
    //  public String toString()
    //{
    //    return new StringBuilder()
    //        .append("SecureMessagingWrapper [")
    //        .append("ssc: ").append(ssc)
    //        .append(", ksEnc: ").append(ksEnc)
    //        .append(", ksMac: ").append(ksMac)
    //        .append(", maxTranceiveLength: ").append(maxTranceiveLength)
    //        .append(", shouldCheckMAC: ").append(shouldCheckMAC)
    //        .append("]")
    //        .toString();
    //}

    //@Override
    //  public int hashCode()
    //{
    //    int prime = 31;
    //    int result = 1;
    //    result = prime * result + ((ksEnc == null) ? 0 : ksEnc.hashCode());
    //    result = prime * result + ((ksMac == null) ? 0 : ksMac.hashCode());
    //    result = prime * result + maxTranceiveLength;
    //    result = prime * result + (shouldCheckMAC ? 1231 : 1237);
    //    result = prime * result + (int)(ssc ^ (ssc >>> 32));
    //    return result;
    //}

    //@Override
    //  public bool equals(Object obj)
    //{
    //    if (this == obj)
    //    {
    //        return true;
    //    }
    //    if (obj == null)
    //    {
    //        return false;
    //    }
    //    if (GetType() != obj.getClass())
    //    {
    //        return false;
    //    }

    //    SecureMessagingWrapper other = (SecureMessagingWrapper)obj;
    //    if (ksEnc == null)
    //    {
    //        if (other.ksEnc != null)
    //        {
    //            return false;
    //        }
    //    }
    //    else if (!ksEnc.equals(other.ksEnc))
    //    {
    //        return false;
    //    }
    //    if (ksMac == null)
    //    {
    //        if (other.ksMac != null)
    //        {
    //            return false;
    //        }
    //    }
    //    else if (!ksMac.equals(other.ksMac))
    //    {
    //        return false;
    //    }
    //    if (maxTranceiveLength != other.maxTranceiveLength)
    //    {
    //        return false;
    //    }
    //    if (shouldCheckMAC != other.shouldCheckMAC)
    //    {
    //        return false;
    //    }

    //    return ssc == other.ssc;
    //}
    //}

}