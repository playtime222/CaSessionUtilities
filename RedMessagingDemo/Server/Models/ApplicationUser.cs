using Microsoft.AspNetCore.Identity;

namespace RedMessagingDemo.Server.Models;

public class ApplicationUser : IdentityUser
{
}


public class FakeApiToken
{
    public long Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public string Token { get; set; }
}
