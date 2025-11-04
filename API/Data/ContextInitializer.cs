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
                var superAdmin = new AppUser
                {
                    Name = "IAAdmin",
                    UserName = SD.SuperAdminUsername,
                    Email = SD.SuperAdminEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                await userManager.CreateAsync(superAdmin, SD.DefaultPassword);
                await userManager.AddToRolesAsync(superAdmin, [SD.AdminRole, SD.UserRole, SD.ModeratorRole]);
                await userManager.AddClaimsAsync(superAdmin, new Claim[]
                {
                    new Claim(SD.Email, superAdmin.Email),
                });

                // 2. John (Admin)
                var john = new AppUser
                {
                    Name = "John",
                    UserName = "john",
                    Email = "john@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(john, SD.DefaultPassword);
                await userManager.AddToRolesAsync(john, [SD.AdminRole, SD.UserRole, SD.ModeratorRole]);

                // 3. Moderator
                var moderator = new AppUser
                {
                    Name = "Moderator",
                    UserName = "moderator",
                    Email = "moderator@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(moderator, SD.DefaultPassword);
                await userManager.AddToRolesAsync(moderator, [SD.UserRole, SD.ModeratorRole]);

                // 4. Les
                var les = new AppUser
                {
                    Name = "LES",
                    UserName = "les",
                    Email = "les@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(les, SD.DefaultPassword);
                await userManager.AddToRoleAsync(les, SD.UserRole);

                // 5. Peter
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

                // 6. Tom
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

                // 7. Barb
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

                // 8. vipuser
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

                // 9. Mary
                var mary = new AppUser
                {
                    Name = "Mary",
                    Email = "mary@example.com",
                    UserName = "mary",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(mary, SD.DefaultPassword);
                await userManager.AddToRoleAsync(mary, SD.UserRole);

                // 10. Sarah
                var sarah = new AppUser
                {
                    Name = "Sarah",
                    Email = "sarah@example.com",
                    UserName = "sarah",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(sarah, SD.DefaultPassword);
                await userManager.AddToRoleAsync(sarah, SD.UserRole);

                // 11. Alice
                var alice = new AppUser
                {
                    Name = "Alice",
                    Email = "alice@example.com",
                    UserName = "alice",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(alice, SD.DefaultPassword);
                await userManager.AddToRoleAsync(alice, SD.UserRole);

                // 12. Bob
                var bob = new AppUser
                {
                    Name = "Bob",
                    Email = "bob@example.com",
                    UserName = "bob",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(bob, SD.DefaultPassword);
                await userManager.AddToRoleAsync(bob, SD.UserRole);

                // 13. Eve
                var eve = new AppUser
                {
                    Name = "Eve",
                    Email = "eve@example.com",
                    UserName = "eve",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(eve, SD.DefaultPassword);
                await userManager.AddToRoleAsync(eve, SD.UserRole);

                // 14. Frank
                var frank = new AppUser
                {
                    Name = "Frank",
                    Email = "frank@example.com",
                    UserName = "frank",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(frank, SD.DefaultPassword);
                await userManager.AddToRoleAsync(frank, SD.UserRole);

                // 15. Grace
                var grace = new AppUser
                {
                    Name = "Grace",
                    Email = "grace@example.com",
                    UserName = "grace",
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(grace, SD.DefaultPassword);
                await userManager.AddToRoleAsync(grace, SD.UserRole);
            }
        }
    }
}
