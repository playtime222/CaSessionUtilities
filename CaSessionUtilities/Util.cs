using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilities;

public static class Util
{
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


    public const int ENC_MODE = 1;
    public const int MAC_MODE = 2;

    public static bool equalsIgnoreCase(this string thiz, string value) => thiz.Equals(value, StringComparison.InvariantCultureIgnoreCase);
    public static bool startsWith(this string thiz, string value) => thiz.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
    public static bool equals(this string thiz, string value) => thiz.equalsIgnoreCase(value);


    /**
     * Derives a shared key.
     *
     * @param keySeed the shared secret, as octets
     * @param cipherAlg in Java mnemonic notation (for example "DESede", "AES")
     * @param keyLength length in bits
     * @param nonce optional nonce or <code>null</code>
     * @param mode the mode either {@code ENC}, {@code MAC}, or {@code PACE} mode
     * @param paceKeyReference Key Reference For Pace Protocol
     *
     * @return the derived key
     *
     * @throws GeneralSecurityException if something went wrong
     */
    public static byte[] deriveKey(byte[] keySeed, string cipherAlg, int keyLength, int mode)
    {
        var digest = getDigest(cipherAlg, keyLength);
        digest.BlockUpdate(keySeed, 0, keySeed.Length);
        digest.BlockUpdate(new byte[] { 0x00, 0x00, 0x00, (byte)mode }, 0, 4);
        var hashResult = new byte[digest.GetDigestSize()];
        digest.DoFinal(hashResult, 0);

        byte[] keyBytes = null;
        if ("DESede".equalsIgnoreCase(cipherAlg) || "3DES".equalsIgnoreCase(cipherAlg))
        {
            /* TR-SAC 1.01, 4.2.1. */
            switch (keyLength)
            {
                case 112:
                case 128:
                    keyBytes = new byte[24];
                    Array.Copy(hashResult, 0, keyBytes, 0, 8); /* E  (octets 1 to 8) */
                    Array.Copy(hashResult, 8, keyBytes, 8, 8); /* D  (octets 9 to 16) */
                    Array.Copy(hashResult, 0, keyBytes, 16, 8); /* E (again octets 1 to 8, i.e. 112-bit 3DES key) */
                    return keyBytes;
                default:
                    throw new InvalidOperationException("DESede with 128-bit key length only");
            }
        }
        else if ("AES".equalsIgnoreCase(cipherAlg) || cipherAlg.startsWith("AES"))
        {
            /* TR-SAC 1.01, 4.2.2. */
            switch (keyLength)
            {
                case 128:
                case 192:
                case 256:
                    keyBytes = new byte[keyLength / 8]; /* NOTE: 256 = 32 * 8 */
                    Array.Copy(hashResult, 0, keyBytes, 0, keyLength / 8);
                    return keyBytes;
                default:
                    throw new InvalidOperationException("KDF can only use AES with 128-bit, 192-bit key or 256-bit length");
            }
        }

        return keyBytes;
    }

    private static GeneralDigest getDigest(string cipherAlg, int keyLength)
    {
        if ("DESede".equals(cipherAlg) || "AES-128".equals(cipherAlg))
            return new Sha1Digest();
        if ("AES".equals(cipherAlg) && keyLength == 128)
            return new Sha1Digest();
        if ("AES-256".equals(cipherAlg) || "AES-192".equals(cipherAlg))
            return new Sha256Digest();
        if ("AES".equals(cipherAlg) && (keyLength == 192 || keyLength == 256))
            return new Sha256Digest();

        throw new InvalidOperationException("Unsupported cipher algorithm or key length.");
    }
}