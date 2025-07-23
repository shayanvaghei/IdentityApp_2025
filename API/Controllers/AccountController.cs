using API.DTOs;
using API.DTOs.Account;
using API.Extensions;
using API.Models;
using API.Services.IServices;
using API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : ApiCoreController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [Authorize]
        [HttpGet("refresh-appuser")]
        public async Task<ActionResult<AppUserDto>> RefreshAppUser()
        {
            var user = await _userManager.Users
                .Where(x => x.Id == User.GetUserId())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                RemoveJwtCookie();
                return Unauthorized(new ApiResponse(401));
            }

            return CreateAppUserDto(user);
        }

        [HttpGet("auth-status")]
        public IActionResult IsLoggedIn()
        {
            return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.Users
                .Where(x => x.UserName == model.UserName)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                user = await _userManager.Users
                    .Where(x => x.Email == model.UserName)
                    .FirstOrDefaultAsync();
            }

            if (user == null) return Unauthorized(new ApiResponse(401, message: "Invalid username or password"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

            if (!result.Succeeded)
            {
                RemoveJwtCookie();

                if (result.IsLockedOut)
                {
                    return Unauthorized(new ApiResponse(401, title: "Account Locked",
                        message: SD.AccountLockedMessage(user.LockoutEnd.Value.DateTime), isHtmlEnabled: true, displayByDefault: true));
                }

                return Unauthorized(new ApiResponse(401, message: "Invalid username or password"));
            }

            return CreateAppUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest(new ApiResponse(400,
                    message: $"An account has been registered with '{model.Email}'. Please try using another email address"));
            }

            if (await CheckNameExistsAsync(model.Name))
            {
                return BadRequest(new ApiResponse(400,
                    message: $"An account has been registered with '{model.Name}'. Please try using another name (username)"));
            }

            var userToAdd = new AppUser
            {
                Name = model.Name,
                UserName = model.Name.ToLower(),
                Email = model.Email,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new ApiResponse(200, message: "Your account has been created, you can login"));
        }

        [HttpGet("name-taken")]
        public async Task<IActionResult> NameTaken([FromQuery] string name)
        {
            return Ok(new { IsTaken = await CheckNameExistsAsync(name) });
        }

        [HttpGet("email-taken")]
        public async Task<IActionResult> EmailTaken([FromQuery] string email)
        {
            return Ok(new { IsTaken = await CheckEmailExistsAsync(email) });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            RemoveJwtCookie();
            return NoContent();
        }

        #region Private Methods
        private AppUserDto CreateAppUserDto(AppUser user)
        {
            string jwt = _tokenService.CreateJWT(user);
            SetJWTCookie(jwt);

            return new AppUserDto
            {
                Name = user.Name,
                JWT = jwt,
            };
        }
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
        private void RemoveJwtCookie()
        {
            Response.Cookies.Delete(SD.IdentityAppCookie);
        }
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email);
        }
        private async Task<bool> CheckNameExistsAsync(string name)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == name.ToLower());
        }
        #endregion
    }
}
