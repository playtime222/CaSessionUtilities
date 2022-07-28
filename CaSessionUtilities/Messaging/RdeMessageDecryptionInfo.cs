namespace CaSessionUtilities.Messaging;

public class RdeMessageDecryptionInfo
{
    /// <summary>
    /// Display name for the receiver's document used during enrolment
    /// </summary>
    public string DocumentDisplayName { get; set; }

    /// <summary>
    /// From the DG14 info.
    /// </summary>
    public string CaProtocolOid { get; set; }

    /// <summary>
    /// Hex encoded
    /// AKA Ephemeral Key Z
    /// </summary>
    public string PcdPublicKey { get; set; } //from EAC CA session

    /// <summary>
    /// Hex encoded
    /// Encrypted RB command 
    /// </summary>
    public string Command { get; set; } //from EAC CA session


#if DEBUG
    /// <summary>
    /// TODO NB this member contains secret information that should not be present in a production system
    /// </summary>
    public RdeMessageDebugInfo DebugInfo { get; set; }
#endif
}