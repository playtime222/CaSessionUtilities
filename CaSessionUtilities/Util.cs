using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

public static class Util
{
    //public static int getPaddedLength(int bufferSize, int BLOCK_SIZE)
    //{
    //    return ((bufferSize + BLOCK_SIZE) / BLOCK_SIZE) * BLOCK_SIZE;
    //}

    public static byte[] pad(byte[] input, int blockSize) => input.GetPaddedArrayMethod2(blockSize);

    //public static byte[] pad(byte[] input, int start, int len, int blockSize) =>
    //    throw new Exception();  

    public static string[] SplitEvery(this string thiz, int partLength)
    {
        if (thiz == null)
            throw new ArgumentNullException(nameof(thiz));

        if (partLength < 1)
            throw new ArgumentException("Part length has to be positive.", nameof(partLength));

        var result = new List<string>();

        for (var i = 0; i < thiz.Length; i += partLength)
            result.Add(thiz.Substring(i, Math.Min(partLength, thiz.Length - i)));

        return result.ToArray();
    }

    public static string PrettyHexFormat(this byte[] thiz)
        => string.Join("-", Hex.ToHexString(thiz).SplitEvery(16));

}