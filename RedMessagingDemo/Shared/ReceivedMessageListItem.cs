namespace RedMessagingDemo.Shared;

public class ReceivedMessageListItem
{
    public long Id{ get; set; }
    public string DocumentDisplayName{ get; set; }
    public DateTime WhenSent{ get; set; }
    public string SenderEmail { get; set; }
    public string Note { get; set; }
}