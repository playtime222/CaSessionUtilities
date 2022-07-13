namespace RedMessagingDemo.Shared;

public class ReceivedMessageListItem
{
    public virtual long Id{ get; set; }
    public virtual DateTime WhenSent{ get; set; }
    public virtual string SenderEmail { get; set; }
    public virtual string Note { get; set; }
}