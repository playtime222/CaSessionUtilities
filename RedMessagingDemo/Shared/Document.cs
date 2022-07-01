namespace RedMessagingDemo.Shared;

public class Document
{
    public long Id { get; set; }

    //@ManyToOne
    public long Owner { get; set; }

    ///@Column(nullable = true)
    //TODO Optional mnemonic only
    public String DisplayName { get; set; }

    public byte[] DataGroup14 { get; set; }

    public string CaProtocolOid { get; set; }
    public string CaProtocolPublicKey { get; set; }

    //TODO allow multiple
    public int FileId { get; set; } //Field on document which is the target of the RB call.
    public string FileContents { get; set; } //Field on document which is the target of the RB call.
    public int FileReadLength { get; set; }
}


public record DocumentList
{
    public DocumentListItem[] Items {get;set;}
}

public record DocumentListItem
{
    public long Id { get; set; }
    public string DisplayName { get; set; }
}



