using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class AppUserRoleBridge : IdentityUserRole<int>
    {
        // Navigations
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
