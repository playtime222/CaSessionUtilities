﻿using Microsoft.EntityFrameworkCore;
using RedMessagingDemo.Server.Data;
using RedMessagingDemo.Shared;

namespace RedMessagingDemo.Server.Commands;

public class FindReceiverDocumentsCommand
{
    private readonly ApplicationDbContext _Db;

    public FindReceiverDocumentsCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public async Task<ReceiverDocumentList> ExecuteAsync()
        => new()
        {
            Items = await _Db.Documents
                .Select(x => new ReceiverDocumentListItem { Id = x.Id, EmailAddress = x.Owner.Email, DisplayName = x.DisplayName })
                .ToArrayAsync()
        };

}