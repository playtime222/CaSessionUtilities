namespace CaSessionUtilities.Wrapping.Implementation;

public static class PaddingIso9797
{
    private const byte PaddingStart = 0x80; //Appended single bit
    private const byte Padding = 0x00;

    /// <summary>
    /// Always append padding marker, then pad.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="blockSize"></param>
    /// <returns></returns>
    public static byte[] GetPaddedArrayMethod2(this byte[] input, int blockSize)
        => input.GetPaddedArrayMethod2(0, input.Length, blockSize);

    public static byte[] GetPaddedArrayMethod2(this byte[] input, int offset, int length, int blockSize)
    {
        var s = new MemoryStream();
        s.Write(input, offset, length);
        s.WriteByte(PaddingStart);
        while (s.Length % blockSize != 0)
            s.WriteByte(Padding);

        return s.ToArray();
    }


    /// <summary>
    /// No padding added if already aligned with block inputSize, otherwise use method 2.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="blockSize"></param>
    /// <returns></returns>
    public static byte[] GetPaddedArrayMethod1(this byte[] input, int blockSize)
        => SizeAlignsWithBlockSize(input.Length, blockSize) ? input : input.GetPaddedArrayMethod2(blockSize);

    private static int GetPaddedLengthMethod1(int inputSize, int blockSize) => (inputSize + blockSize) / blockSize * blockSize;

    private static bool SizeAlignsWithBlockSize(int inputSize, int blockSize) => GetPaddedLengthMethod1(inputSize, blockSize) == inputSize;
}