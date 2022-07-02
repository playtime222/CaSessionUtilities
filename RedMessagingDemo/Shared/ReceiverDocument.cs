namespace RedMessagingDemo.Shared;


/// <summary>
/// AKA RDE Certificate contents
/// </summary>
public record ReceiverDocument
{
    public CaSessionArgs CaSessionArgs { get; init; }
    public int FileShortId { get; init; }
    public string FileContentBase64 { get; init; }

    /// <summary>
    /// Max is 256 cos max transceive length
    /// </summary>
    public int ReadLength { get; init; }
}