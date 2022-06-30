namespace RedMessagingDemo.Server.Models;

public class Message
{
    //@GeneratedValue(strategy = GenerationType.AUTO)
    public virtual long Id { get; set; }
    public virtual DateTime WhenSent{ get; set; }

    //@ManyToOne
    public virtual ApplicationUser FromUser { get; set; }

    //@ManyToOne
    public virtual Document Document { get; set; }
    public virtual String Note { get; set; }
    public virtual byte[] Content { get; set; }
}