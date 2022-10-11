using Microsoft.EntityFrameworkCore;
using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Shared;

namespace RdeMessagingDemo.Server.Commands;

public class ListDocumentsByUserCommand
{
    private readonly ApplicationDbContext _Db;

    public ListDocumentsByUserCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<DocumentList> ExecuteAsync(string userId)
    {
        var docs = await _Db.Documents
            .Where(x => x.Owner.Id == userId)
            .Select(x => new DocumentListItem { Id = x.Id, DisplayName = x.DisplayName })
            .ToArrayAsync();

        return new DocumentList()
        {
            Items = docs
        };
    }
}