namespace CaSessionUtilities.Messaging;

public class RdeMessageDecryptionInfo
{
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
}