using RdeMessagingDemo.Server.Data;
using RdeMessagingDemo.Server.Models;

namespace RdeMessagingDemo.Server.Commands;

public class FindUserFromBearerTokenCommand
{
    private readonly ApplicationDbContext _Db;

    public FindUserFromBearerTokenCommand(ApplicationDbContext db)
    {
        _Db = db;
    }

    public bool TryGet(string headerValue, out ApplicationUser? user)
    {
        if (!headerValue.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
        {
            user = null;
            return false;
        }

        var token = headerValue.Replace("bearer", "", StringComparison.InvariantCultureIgnoreCase).Trim();

        user = _Db.FakeApiTokens.Where(x => x.Token == token)
            .Select(x => x.ApplicationUser).FirstOrDefault();

        return user != null;
    }
}