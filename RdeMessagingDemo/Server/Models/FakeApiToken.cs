namespace RdeMessagingDemo.Server.Models;

public class FakeApiToken
{
    public long Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public string Token { get; set; }
}