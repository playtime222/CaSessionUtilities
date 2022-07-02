namespace RedMessagingDemo.Shared;

public record MessageSendArgs
{
    public long Receiver { get; set; }
    public string MessageBase64 { get; set; }
    public string Note { get; set; }
}