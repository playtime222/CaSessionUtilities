using System.Diagnostics;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

public class AesSecureMessagingWrapperResponseEncoder
{
    public const string DO87_CIPHER = "AES/CBC/NoPadding";

    public const string IV_CIPHER = "AES/ECB/NoPadding"; //Not directly available from Web Browser

    //Metadata
    public const string MAC_ALGO = "AESCMAC";

    public static int BLOCK_SIZE = 16;

    // Plain text block size cos AES and AESCMAC
    private static byte SW1 = 0x90;

    private static byte SW2 = 0;

    private static byte DATA_BLOCK_START_TAG = 0x87;

    private static byte DATA_BLOCK_LENGTH_END_TAG = 0x01;

    public static int MAC_LENGTH = 8;

    public static int MAC_BLOCK_START_TAG = 0x8e;

    private static readonly byte[] RESPONSE_RESULT_BLOCK = {0x99,0x02,SW1,SW2};

    private readonly MemoryStream _Result = new();

    private readonly IBufferedCipher _Cipher;

    private readonly IMac _Mac;

    private static byte[] SSC = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 };

    // 0 when CA session started, first command is 1, first response is 2.
    public AesSecureMessagingWrapperResponseEncoder(byte[] ksEnc, byte[] ksMac)
    {
        _Cipher = CipherUtilities.GetCipher(DO87_CIPHER);
        var secretKey = new KeyParameter(ksEnc);
        var iv = getIv(ksEnc);
        _Cipher.Init(true, new ParametersWithIV(secretKey, iv));

        _Mac = new CMac(new AesEngine(), 128);
        var macKey = new KeyParameter(ksMac);
        _Mac.Init(macKey);
    }

    public static byte[] getIv(byte[] ksEnc)
    {
        var sscIVCipher = CipherUtilities.GetCipher(IV_CIPHER);
        sscIVCipher.Init(true, new KeyParameter(ksEnc));
        return sscIVCipher.DoFinal(SSC);
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
        
        _Mac.BlockUpdate(SSC, 0, SSC.Length);
        var padded = _Result.ToArray().GetPaddedArrayMethod2(BLOCK_SIZE);
        _Mac.BlockUpdate(padded, 0, padded.Length);
        var macValue = new byte[_Mac.GetMacSize()];

        _Mac.DoFinal(macValue, 0);
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

        var encodedData = GetEncodedData(response);
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

    private byte[] GetEncodedData(byte[] response)
    {
        if (response.Length == 0)
            return response;

        return _Cipher.DoFinal(response.GetPaddedArrayMethod1(BLOCK_SIZE));
    }



}