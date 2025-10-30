using API.Data;
using API.DTOs;
using API.DTOs.Account;
using API.Models;
using API.Services.IServices;
using API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCoreController : ControllerBase
    {
        private Context _context;
        private IConfiguration _config;
        private IServiceUnitOfWork _service;
        private UserManager<AppUser> _userManager;
        protected IConfiguration Configuration => _config ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected Context Context => _context ??= HttpContext.RequestServices.GetService<Context>();
        protected IServiceUnitOfWork Services => _service ??= HttpContext.RequestServices.GetService<IServiceUnitOfWork>();
        protected UserManager<AppUser> UserManager => _userManager ??= HttpContext.RequestServices.GetService<UserManager<AppUser>>();

        protected async Task<string> UserPasswordValidationAsync(AppUser user, string password)
        {
            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                RemoveJwtCookie();
                return SD.AccountLockedMessage(user.LockoutEnd.Value.DateTime);
            }

            var isCurrentPasswordValid = await UserManager.CheckPasswordAsync(user, password);
            if (!isCurrentPasswordValid)
            {
                // Increament AccessFailedCount of the AspNetUser by 1
                await UserManager.AccessFailedAsync(user);

                if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
                {
                    RemoveJwtCookie();
                    return SD.AccountLockedMessage(user.LockoutEnd.Value.DateTime);
                }

                int remaining = SD.MaxFailedAccessAttempts - user.AccessFailedCount;
                return $"Invalid password. You have {remaining} attempt(s) left.";
            }

            user.LockoutEnd = null;
            await UserManager.ResetAccessFailedCountAsync(user);
            await Context.SaveChangesAsync();

            return null;
        }
        protected async Task<AppUserDto> CreateAppUserDtoAsync(AppUser user)
        {
            string jwt = await Services.TokenService.CreateJWTAsync(user);
            SetJWTCookie(jwt);

            return new AppUserDto
            {
                Name = user.Name,
                JWT = jwt,
            };
        }
        protected void RemoveJwtCookie()
        {
            Response.Cookies.Delete(SD.IdentityAppCookie);
        }
        protected async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await UserManager.Users.AnyAsync(x => x.Email == email);
        }
        protected async Task<bool> CheckNameExistsAsync(string name)
        {
            return await UserManager.Users.AnyAsync(x => x.UserName == name.ToLower());
        }
        protected async Task<bool> SendConfirmEmailAsync(AppUser user)
        {
            var userToken = await Context.AppUserTokens
                .Where(x => x.UserId == user.Id && x.Name == SD.EC)
                .FirstOrDefaultAsync();

            var tokenExpiresInMinuest = TokenExpiresInMinutes();

            if (userToken == null)
            {
                var userTokenToAdd = new AppUserToken
                {
                    UserId = user.Id,
                    Name = SD.EC,
                    Value = SD.GenerateRandomString(),
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpiresInMinuest),
                    LoginProvider = string.Empty
                };

                Context.AppUserTokens.Add(userTokenToAdd);
                userToken = userTokenToAdd;
            }
            else
            {
                userToken.Value = SD.GenerateRandomString();
                userToken.Expires = DateTime.UtcNow.AddMinutes(tokenExpiresInMinuest);
            }

            using StreamReader streamReader = System.IO.File.OpenText("EmailTemplates/confirm_email.html");
            string htmlBody = streamReader.ReadToEnd();

            string messageBody = string.Format(htmlBody, GetClientUrl(), user.Name, user.UserName, user.Email,
                userToken.Value, tokenExpiresInMinuest);
            var emailSend = new EmailSendDto(user.Email, "Verify your email address", messageBody);

            if (await Services.EmailService.SendEmailAsync(emailSend))
            {
                await Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        protected int TokenExpiresInMinutes()
        {
            return int.Parse(Configuration["Email:TokenExpiresInMinutes"]);
        }

        protected string GetClientUrl()
        {
            return Configuration["JWT:ClientUrl"];
        }

        protected void PauseResponse(double sec = 1.3)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(sec));
                return 42;
            });
            t.Wait();
        }

        #region Private Methods
        private void SetJWTCookie(string jwt)
        {
            var cookieOptions = new CookieOptions
            {
                IsEssential = true,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(int.Parse(Configuration["JWT:ExpiresInDays"])),
                SameSite = SameSiteMode.None,
            };

            Response.Cookies.Append(SD.IdentityAppCookie, jwt, cookieOptions);
        }
        #endregion
    }
}
