using Microsoft.EntityFrameworkCore;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Shared;

namespace RedMessagingDemo.Server.Commands;

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
            .Select(x => new ReceivedMessageListItem { Id = x.Id, Note = x.Note, SenderEmail = x.FromUser.Email, WhenSent = x.WhenSent })
            .ToArrayAsync();

        return new ReceivedMessageList()
        {
            Items = items
        };
    }
}