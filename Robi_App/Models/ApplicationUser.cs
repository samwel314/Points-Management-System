using Microsoft.AspNetCore.Identity;

namespace Robi_App.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string TemporaryPassword { get; set; } = null!;       
    }
}
