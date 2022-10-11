using Microsoft.EntityFrameworkCore;
using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Shared;

namespace RdeMessagingDemo.Server.Commands;

public class ListMessagesByUserCommand
{
    private readonly ApplicationDbContext _Db;

    public ListMessagesByUserCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<ReceivedMessageList> Execute(string userId)
    {
        var items = await _Db.Messages
            .Where(x => x.Document.Owner.Id == userId)
            .Select(x => new ReceivedMessageListItem { Id = x.Id, Note = x.Note, SenderEmail = x.FromUser.Email, WhenSent = x.WhenSent.ToString("u"), DocumentDisplayName=x.Document.DisplayName
            })
            .ToArrayAsync();

        return new ReceivedMessageList()
        {
            Items = items
        };
    }
}