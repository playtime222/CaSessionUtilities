using CaSessionUtilities.Wrapping;

namespace CaSessionUtilities;

public record RdeEACCAResult
{
    public byte[] PcdPublicKey { get; }
    public SecureMessagingWrapper Wrapper { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pcdPublicKey">Something that can be encoded that Java understands</param>
    /// <param name="wrapper"></param>
    public RdeEACCAResult(byte[] pcdPublicKey, SecureMessagingWrapper wrapper)
    {
        PcdPublicKey = pcdPublicKey;
        Wrapper = wrapper;
    }
}