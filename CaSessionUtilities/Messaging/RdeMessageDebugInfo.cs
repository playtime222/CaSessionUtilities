using CaSessionUtilities.Wrapping;

namespace CaSessionUtilities.Messaging;


#if DEBUG

public class RdeMessageDebugInfo
{
    public string SharedSecretHex { get; set; }
    public string ReadBinaryResponseHex { get; set; }
    public SecureMessagingWrapperDebugInfo CaWrapperDebugInfo { get; set; }
    public string WrappedResponseHex { get; set; }
}

#endif