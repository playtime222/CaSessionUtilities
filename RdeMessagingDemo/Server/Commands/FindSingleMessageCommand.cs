using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities.Encoders;
using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Server.Models;
using RdeMessagingDemo.Shared;

namespace RdeMessagingDemo.Server.Commands;

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
            .Select(x => new ReceivedMessage
            {
                Id = x.Id,
                Note = x.Note,
                SenderEmail = x.FromUser.Email,
                WhenSent = x.WhenSent.ToString("u"),
                ContentBase64 = Base64UrlEncoder.Encode(x.Content)
            })
            .SingleOrDefaultAsync();
    }
}