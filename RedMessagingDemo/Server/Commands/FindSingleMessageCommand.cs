using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Server.Models;
using RedMessagingDemo.Shared;

namespace RedMessagingDemo.Server.Commands;

public class FindSingleMessageCommand
{
    private readonly ApplicationDbContext _Db;

    public FindSingleMessageCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<ReceivedMessage?> ExecuteAsync(long id, string userId)
    {
        return await _Db.Messages
            .Where(x => x.Document.Owner.Id == userId && x.Id == id)
            .Select(x => ReceivedMessage(x))
            .SingleOrDefaultAsync();
    }

    private static ReceivedMessage ReceivedMessage(Message x)
    {
        var hex = Hex.ToHexString(x.Content); //Debug
        return new ReceivedMessage { Id = x.Id, Note = x.Note, SenderEmail = x.FromUser.Email, WhenSent = x.WhenSent.ToString("u"), 
            ContentBase64 = Base64UrlEncoder.Encode(x.Content) }; //TODO stop url encoding!
    }
}