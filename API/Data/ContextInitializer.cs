using API.Models;
using API.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Data
{
    public static class ContextInitializer
    {
        public static async Task InitializeAsync(Context context, 
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            // Roles
            if (!roleManager.Roles.Any())
            {
                foreach (var role in SD.Roles)
                {
                    await roleManager.CreateAsync(new AppRole { Name = role });
                }
            }

            if (!userManager.Users.Any())
            {
                // 1. Super Admin
                var admin = new AppUser
                {
                    Name = "IA Admin",
                    UserName = SD.SuperAdminUsername,
                    Email = SD.SuperAdminEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                await userManager.CreateAsync(admin, SD.DefaultPassword);
                await userManager.AddToRolesAsync(admin, [SD.AdminRole, SD.UserRole, SD.ModeratorRole]);
                await userManager.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(SD.Email, admin.Email),
                });

                // 2. Moderator
                var moderator = new AppUser
                {
                    Name = "Moderator",
                    UserName = "moderator",
                    Email = "moderator@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(moderator, SD.DefaultPassword);
                await userManager.AddToRoleAsync(moderator, SD.ModeratorRole);

                // 3. John
                var john = new AppUser
                {
                    Name = "JOHN",
                    UserName = "john",
                    Email = "john@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(john, SD.DefaultPassword);
                await userManager.AddToRoleAsync(john, SD.UserRole);

                // 3. Peter
                var peter = new AppUser
                {
                    Name = "peTer",
                    UserName = "peter",
                    Email = "peter@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(peter, SD.DefaultPassword);
                await userManager.AddToRoleAsync(peter, SD.UserRole);

                // 4. Tom
                var tom = new AppUser
                {
                    Name = "tom",
                    UserName = "tom",
                    Email = "tom@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(tom, SD.DefaultPassword);
                await userManager.AddToRoleAsync(tom, SD.UserRole);

                // 5. Barb
                var barb = new AppUser
                {
                    Name = "barb",
                    UserName = "barb",
                    Email = "barb@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(barb, SD.DefaultPassword);
                await userManager.AddToRoleAsync(barb, SD.UserRole);

                // vipuser
                var vipuser = new AppUser
                {
                    Name = "vipuser",
                    UserName = "vipuser",
                    Email = "vipuser@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(vipuser, SD.DefaultPassword);
                await userManager.AddToRoleAsync(vipuser, SD.UserRole);
                await userManager.AddClaimsAsync(vipuser, new Claim[]
                {
                    new Claim(SD.Email, vipuser.Email),
                });
            }
        }
    }
}
