using Microsoft.AspNetCore.Identity;

namespace RedMessagingDemo.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual string DocumentEnrollmentId { get; set; }
    }
}