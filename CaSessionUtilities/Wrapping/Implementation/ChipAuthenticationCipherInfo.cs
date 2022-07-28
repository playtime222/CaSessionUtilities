namespace CaSessionUtilities.Wrapping.Implementation;

public class ChipAuthenticationCipherInfo
{

    public ChipAuthenticationCipherInfo(CipherAlgorithm algorithm, int keyLength)
    {
        Algorithm = algorithm;
        KeyLength = keyLength;
    }

    public CipherAlgorithm Algorithm { get; }
    public int KeyLength { get; }
}