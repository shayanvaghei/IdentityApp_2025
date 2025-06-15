using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigations
        public ICollection<AppUserRoleBridge> Roles { get; set; }
    }
}
