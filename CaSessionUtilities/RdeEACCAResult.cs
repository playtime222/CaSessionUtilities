using CaSessionUtilities.Wrapping;

namespace CaSessionUtilities;

public record RdeEACCAResult
{
    public byte[] PcdPublicKey { get; }
    public SecureMessagingWrapper Wrapper { get; }

#if DEBUG
    public RdeEacCaResultDebugInfo DebugInfo { get;}
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pcdPublicKey">Something that can be encoded that Java understands</param>
    /// <param name="wrapper"></param>
    /// <param name="debugInfo"></param>
    public RdeEACCAResult(byte[] pcdPublicKey, SecureMessagingWrapper wrapper
#if DEBUG
        , RdeEacCaResultDebugInfo debugInfo
#endif
        )
    {
        PcdPublicKey = pcdPublicKey;
        Wrapper = wrapper;
        DebugInfo = debugInfo;
    }
}