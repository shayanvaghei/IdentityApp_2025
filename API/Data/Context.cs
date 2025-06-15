using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Context : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, 
                            AppUserRoleBridge, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().HasMany(ur => ur.Roles).WithOne(u => u.User).HasForeignKey(u => u.UserId).IsRequired();
            builder.Entity<AppRole>().HasMany(ur => ur.Users).WithOne(u => u.Role).HasForeignKey(u => u.RoleId).IsRequired();
        }
    }
}
