namespace RdeMessagingDemo.Shared;

public class ReceivedMessage
{
    public long Id { get; set; }
    public string WhenSent { get; set; }
    public string SenderEmail { get; set; }
    public string Note { get; set; }
    public string ContentBase64 { get; set; }
}