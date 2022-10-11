using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RdeMessagingDemo.Server.Models;

namespace RdeMessagingDemo.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<FakeApiToken> FakeApiTokens { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Document> Documents { get; set; }
    }
}