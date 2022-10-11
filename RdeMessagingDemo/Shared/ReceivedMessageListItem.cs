namespace RdeMessagingDemo.Shared;

public class ReceivedMessageListItem
{
    public long Id{ get; set; }
    public string DocumentDisplayName{ get; set; }
    
    //ISO8601 date time
    public string WhenSent{ get; set; }
    public string SenderEmail { get; set; }
    public string Note { get; set; }
}