using Microsoft.AspNetCore.Identity;
using System;

namespace API.Models
{
    public class AppUserToken : IdentityUserToken<int>
    {
        public DateTime Expires { get; set; }

        // Navigation
        public AppUser User { get; set; }
    }
}
