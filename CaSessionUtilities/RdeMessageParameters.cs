using CaSessionUtilities.Messaging;

namespace CaSessionUtilities;

public record RdeMessageParameters
{
    /// <summary>
    /// CA Session PCD Public Key
    /// Used during decrypt
    /// </summary>
    public byte[] EphemeralPublicKey { get; init; }

    /// <summary>
    /// Read binary file command wrapped using a CA Session.
    /// Note a wrapped command is not encrypted - it is encoded and has a MAC appended.
    /// </summary>
    public byte[] WrappedCommand { get; init; }
    
    /// <summary>
    /// Wrapped result of sending the Wrapped Command to the MRTD.
    /// Seed for generating message cipher key
    /// </summary>
    public byte[] WrappedResponse { get; init; }


#if DEBUG
    /// <summary>
    /// Internal information of 
    /// </summary>
    public RdeMessageDebugInfo DebugInfo { get; set; }
#endif
}