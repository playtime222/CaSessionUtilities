using Org.BouncyCastle.Utilities.Encoders;
using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Shared;

namespace RdeMessagingDemo.Server.Commands;

public class FindSingleReceiverDocumentCommand
{
    private readonly ApplicationDbContext _Db;

    public FindSingleReceiverDocumentCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<ReceiverDocument?> ExecuteAsync(long id)
    {
        var doc = await _Db.Documents.FindAsync(id);

        if (doc == null)
            return null;

        return new ReceiverDocument()
        {
            DocumentDisplayName = doc.DisplayName,
            ChipAuthenticationProtocolInfo = new() { ProtocolOid = doc.CaProtocolOid, PublicKeyInfo = new ChipAuthenticationPublicKeyInfo { PublicKeyBase64 = Base64.ToBase64String(Hex.Decode(doc.CaProtocolPublicKey)) } },
            FileContentBase64 = Base64.ToBase64String(doc.FileContent),
            FileId = doc.FileId,
            ReadLength = doc.FileReadLength
        };
    }
}