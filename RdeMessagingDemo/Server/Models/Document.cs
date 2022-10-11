namespace RdeMessagingDemo.Server.Models;

public class Document
{
    public long Id { get; set; }

    //@ManyToOne
    public ApplicationUser Owner { get; set; }

    ///@Column(nullable = true)
    //TODO Optional mnemonic only
    public string DisplayName { get; set; }

    public byte[] DataGroup14 { get; set; }
    public string CaProtocolOid { get; set; }
    public string CaProtocolPublicKey { get; set; }

    //TODO allow multiple
    public int FileId { get; set; } //Field on document which is the target of the RB call.
    public byte[] FileContent { get; set; } //Field on document which is the target of the RB call.
    public int FileReadLength { get; set; }
}