namespace CaSessionUtilities.Wrapping.Implementation;

public class ChipAuthenticationCipherInfo
{
    //public enum Alg
    //{
    //    Aes,
    //    DeSede
    //}

    public ChipAuthenticationCipherInfo(string algorithm, int keyLength)
    {
        Algorithm = algorithm;
        KeyLength = keyLength;
    }

    public string Algorithm { get; }
    public int KeyLength { get; }
}