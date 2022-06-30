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
    
    public int MaxTranceiveLength = 256; //Or could be 65536 byt does that need RB2 and so is out of scope?

    protected SecureMessagingWrapper(byte[] ksEnc, byte[] ksMac, string cipherAlg, string macAlg)
    {
        CipherAlg = cipherAlg;
        MacAlg = macAlg;

        KsEnc = ksEnc;
        KsMac = ksMac;
    }

    //Returns block of BlockSize ending in the bytes of the SSC
    public abstract byte[] GetEncodedSendSequenceCounter(long ssc);

     ///the length to use for padding for the cipher
    public abstract int BlockSize { get; }

    public abstract byte[] CalculateMac(byte[] data);

    public abstract byte[] GetEncodedDataForResponse(byte[] response);
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
        byte[] paddedMaskedHeader = Util.pad(maskedHeader, _Wrapper.BlockSize);

        bool hasDO85 = ((byte)commandAPDU.INS == ISO7816.INS_READ_BINARY2);

        byte[] do8587 = new byte[0];
        byte[] do97 = new byte[0];


        /* Include the expected length, if present. */
        if (le > 0)
        {
            do97 = TLVUtil.wrapDO(0x97, encodeLe(le));
        }

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
        var cc = _Wrapper.CalculateMac(n);

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

        var encSeq = _Wrapper.GetEncodedSendSequenceCounter(SSC);
        Trace.WriteLine($"{"ssc",-10}: {encSeq.PrettyHexFormat()}");

        ms.write(encSeq);
        ms.write(paddedMaskedHeader);
        ms.write(do8587);
        ms.write(do97);
        var n = Util.pad(ms.ToArray(), _Wrapper.BlockSize);
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
    private static byte[] encodeLe(int le)
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