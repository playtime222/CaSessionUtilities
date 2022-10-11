using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Shared;

namespace RdeMessagingDemo.Server.Commands;

public class SendMessageCommand
{
    private readonly ApplicationDbContext _Db;

    public SendMessageCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<ActionResult> ExecuteAsync([FromBody] MessageSendRequestArgs args, string userId)
    {
        var sender = await _Db.ApplicationUsers.FindAsync(userId);
        if (sender == null)
            return new BadRequestObjectResult("Bad sender.");

        var receiver = await _Db.Documents.FindAsync(args.Receiver);
        if (receiver == null)
            return new BadRequestObjectResult("Bad receiver.");

        var msg = new Models.Message()
        {
            Document = receiver,
            Content = Base64.Decode(args.MessageBase64),
            FromUser = sender,
            WhenSent = DateTime.Now,
            Note = args.Note
        };

        await _Db.Messages.AddAsync(msg);
        await _Db.SaveChangesAsync();

        return new OkResult();
    }

}