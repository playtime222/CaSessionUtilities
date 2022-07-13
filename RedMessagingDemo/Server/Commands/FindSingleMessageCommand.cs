using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.Encoders;
using RedMessagingDemo.Server.Data;
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
            .Select(x => new ReceivedMessage { Id = x.Id, Note = x.Note, SenderEmail = x.FromUser.Email, WhenSent = x.WhenSent, ContentBase64 = Base64.ToBase64String(x.Content) })
            .SingleOrDefaultAsync();
    }
}