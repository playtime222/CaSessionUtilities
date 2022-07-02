using Org.BouncyCastle.Crypto.Digests;

namespace CaSessionUtilities.Wrapping.Implementation;

public static class SessionMessagingWrapperKeyUtility
{

    public const int ENC_MODE = 1;
    public const int MAC_MODE = 2;

    public static byte[] DeriveKey(byte[] keySeed, ChipAuthenticationCipherInfo cipherInfo, int mode)
        => DeriveKey(keySeed, cipherInfo.Algorithm, cipherInfo.KeyLength, mode);
    private static byte[] DeriveKey(byte[] keySeed, string cipherAlg, int keyLength, int mode)
    {
        var digest = getDigest(cipherAlg, keyLength);
        digest.BlockUpdate(keySeed, 0, keySeed.Length);
        digest.BlockUpdate(new byte[] { 0x00, 0x00, 0x00, (byte)mode }, 0, 4);
        var hashResult = new byte[digest.GetDigestSize()];
        digest.DoFinal(hashResult, 0);

        if ("DESede".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase) || "3DES".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase))
        {
            /* TR-SAC 1.01, 4.2.1. */
            switch (keyLength)
            {
                case 112:
                case 128:
                    var keyBytes = new byte[24];
                    Array.Copy(hashResult, 0, keyBytes, 0, 8); /* E  (octets 1 to 8) */
                    Array.Copy(hashResult, 8, keyBytes, 8, 8); /* D  (octets 9 to 16) */
                    Array.Copy(hashResult, 0, keyBytes, 16, 8); /* E (again octets 1 to 8, i.e. 112-bit 3DES key) */
                    return keyBytes;
                default:
                    throw new InvalidOperationException("DESede with 128-bit key length only");
            }
        }
        else if ("AES".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase) || cipherAlg.StartsWith("AES", StringComparison.InvariantCultureIgnoreCase))
        {
            /* TR-SAC 1.01, 4.2.2. */
            switch (keyLength)
            {
                case 128:
                case 192:
                case 256:
                    var keyBytes = new byte[keyLength / 8]; /* NOTE: 256 = 32 * 8 */
                    Array.Copy(hashResult, 0, keyBytes, 0, keyBytes.Length);
                    return keyBytes;
                default:
                    throw new InvalidOperationException("KDF can only use AES with 128-bit, 192-bit key or 256-bit length");
            }
        }

        throw new InvalidOperationException();
    }

    private static GeneralDigest getDigest(string cipherAlg, int keyLength)
    {
        if ("DESede".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase) || "AES-128".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase))
            return new Sha1Digest();
        if ("AES".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase) && keyLength == 128)
            return new Sha1Digest();

        if ("AES-256".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase) || "AES-192".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase))
            return new Sha256Digest();
        if ("AES".Equals(cipherAlg, StringComparison.InvariantCultureIgnoreCase) && (keyLength == 192 || keyLength == 256))
            return new Sha256Digest();

        throw new InvalidOperationException("Unsupported cipher algorithm or key length.");
    }
}