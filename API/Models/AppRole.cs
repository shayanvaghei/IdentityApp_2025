using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace API.Models
{
    public class AppRole : IdentityRole<int>
    {
        // Navigations
        public ICollection<AppUserRoleBridge> Users { get; set; }
    }
}
