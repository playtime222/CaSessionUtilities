using System.Diagnostics;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

public class ResponseEncoder
{
    private readonly SecureMessagingWrapper _Wrapper;

    private static byte SW1 = 0x90;

    private static byte SW2 = 0;

    private static byte DATA_BLOCK_START_TAG = 0x87;

    private static byte DATA_BLOCK_LENGTH_END_TAG = 0x01;

    public static int MAC_LENGTH = 8;

    public static int MAC_BLOCK_START_TAG = 0x8e;

    private static readonly byte[] RESPONSE_RESULT_BLOCK = {0x99,0x02,SW1,SW2};

    private readonly MemoryStream _Result = new();

    private const long SSC = 2;

    // 0 when CA session started, first command is 1, first response is 2.
    public ResponseEncoder(SecureMessagingWrapper wrapper)
    {
        _Wrapper = wrapper;
    }

    public byte[] Write(byte[] response)
    {
        Trace.WriteLine(("Response:" + Hex.ToHexString(response)));
        WriteDo87(response);
        WriteDo99();
        WriteMac();
        _Result.Write(new byte[] {
            SW1,
            SW2});
        // Again...
        return _Result.ToArray();
    }

    private void WriteMac()
    {
        Trace.WriteLine("MAC this: " + Hex.ToHexString(_Result.ToArray()));
        var joined = Arrays.Concatenate(_Wrapper.GetEncodedSendSequenceCounter(SSC), _Result.ToArray().GetPaddedArrayMethod2(_Wrapper.BlockSize));
        var macValue = _Wrapper.CalculateMac(joined);
        Trace.WriteLine("MAC     : " + Hex.ToHexString(macValue));
        _Result.Write(new [] { (byte)(MAC_BLOCK_START_TAG), (byte)MAC_LENGTH });
        _Result.Write(macValue, 0, MAC_LENGTH);
    }

    private void WriteDo99()
    {
        _Result.Write(RESPONSE_RESULT_BLOCK);
    }

    private void WriteDo87(byte[] response)
    {
        if (response.Length == 0)
            return;

        var encodedData = _Wrapper.GetEncodedDataForResponse(response.GetPaddedArrayMethod1(_Wrapper.BlockSize));
        _Result.Write(new[] { DATA_BLOCK_START_TAG });
        _Result.Write(GetEncodedDo87Size(encodedData.Length));
        _Result.Write(encodedData);
    }

    //// TODO make private. Only public for tests.
    public byte[] GetEncodedDo87Size(int paddedDo87Length)
    {
        int MIN_LONG_FORM_SIZE = 128;
        var actualLength = (paddedDo87Length + 1);
        // Cos of the 0x01 tag
        // Short form
        if ((actualLength < MIN_LONG_FORM_SIZE))
        {
            return new byte[] {
                ((byte)(actualLength)),
                DATA_BLOCK_LENGTH_END_TAG};
        }

        // 1 or 2 byte Long form
        var lenOfLen = actualLength > 0xFF ? 2 : 1;
        var result = new byte[lenOfLen + 2];
        result[0] = ((byte)((MIN_LONG_FORM_SIZE + lenOfLen)));
        var p = 1;
        for (var i = (lenOfLen - 1); (i >= 0); i--)
            //>> was Java >>> unsigned right shift
            result[p++] = (byte)((actualLength >> (i * 8)) & 0xFF);


        result[p] = DATA_BLOCK_LENGTH_END_TAG;
        return result;
    }
}