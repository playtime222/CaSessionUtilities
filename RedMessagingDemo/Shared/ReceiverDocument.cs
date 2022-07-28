namespace RedMessagingDemo.Shared;


/// <summary>
/// AKA RDE Certificate contents
/// </summary>
public class ReceiverDocument
{
    public string DocumentDisplayName { get; set; }

    public ChipAuthenticationProtocolInfo ChipAuthenticationProtocolInfo { get; init; }
    public int FileId { get; init; }
    public string FileContentBase64 { get; init; }

    /// <summary>
    /// Max is 256 cos max transceive length
    /// </summary>
    public int ReadLength { get; init; }
}