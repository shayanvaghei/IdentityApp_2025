using API.DTOs;
using API.DTOs.Account;
using API.Extensions;
using API.Models;
using API.Services.IServices;
using API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : ApiCoreController
    {
        [Authorize]
        [HttpGet("refresh-appuser")]
        public async Task<ActionResult<AppUserDto>> RefreshAppUser()
        {
            var user = await UserManager.Users
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
            var user = await UserManager.Users
                .Where(x => x.UserName == model.UserName)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                user = await UserManager.Users
                    .Where(x => x.Email == model.UserName)
                    .FirstOrDefaultAsync();
            }

            if (user == null) return Unauthorized(new ApiResponse(401, message: "Invalid username or password", displayByDefault: true));

            var message = await UserPasswordValidationAsync(user, model.Password);
            if (!string.IsNullOrEmpty(message))
            {
                return Unauthorized(new ApiResponse(401, message: message, displayByDefault: true, isHtmlEnabled: true));
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(new ApiResponse(401, title: SM.T_ConfirmEmailFirst, message: SM.M_ConfirmEmailFirst,
                    displayByDefault: true));
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
                EmailConfirmed = false,
                LockoutEnabled = true
            };

            var result = await UserManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            try
            {
                if (await SendConfirmEmailAsync(userToAdd))
                {
                    return Ok(new ApiResponse(201, title: SM.T_AccountCreated, message: SM.M_AccountCreated));
                }

                return BadRequest(new ApiResponse(400, title: SM.T_EmailSentFailed,
                   message: SM.M_EmailSentFailed, displayByDefault: true));
            }
            catch (Exception)
            {
                return BadRequest(new ApiResponse(400, title: SM.T_EmailSentFailed,
                    message: SM.M_EmailSentFailed, displayByDefault: true));
            }
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

        [HttpPut("confirm-email")]
        public async Task<ActionResult<ApiResponse>> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401, title: SM.T_InvallidToken, message: SM.M_InavlidToken,
                    displayByDefault: true));
            }

            if (!user.IsActive)
            {
                return Unauthorized(new ApiResponse(401, title: SM.T_AccountSuspended, message: SM.M_AccountSuspended,
                    displayByDefault: true));
            }

            if (user.EmailConfirmed == true)
            {
                return BadRequest(new ApiResponse(400, title: SM.T_AccountWasConfirmed, message: SM.M_AccountWasConfirmed,
                    displayByDefault: true));
            }

            var appUserToken = await Context.AppUserTokens
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Name == SD.EC && x.Value == model.Token);
            if (appUserToken == null || appUserToken.Expires <= DateTime.UtcNow)
            {
                if (appUserToken != null)
                {
                    Context.AppUserTokens.Remove(appUserToken);
                    await Context.SaveChangesAsync();
                }

                return Unauthorized(new ApiResponse(401, title: SM.T_InvallidToken, message: SM.M_InavlidToken,
                   displayByDefault: true));
            }

            Context.AppUserTokens.Remove(appUserToken);
            user.EmailConfirmed = true;
            await Context.SaveChangesAsync();

            return Ok(new ApiResponse(200, title: SM.T_EmailConfirmed, message: SM.M_EmailConfirmed));
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<ActionResult<ApiResponse>> ResendConfirmationEmail(EmailDto model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // sending a vague response with a fake delay
                PauseResponse();
                return Ok(new ApiResponse(200, title: SM.T_EmailSent, message: SM.M_ConfirmEmailSend));
            }

            if (!user.IsActive)
            {
                return Unauthorized(new ApiResponse(401, title: SM.T_AccountSuspended, message: SM.M_AccountSuspended,
                    displayByDefault: true));
            }

            if (user.EmailConfirmed == true)
            {
                return BadRequest(new ApiResponse(400, title: SM.T_AccountWasConfirmed, message: SM.M_AccountWasConfirmed,
                    displayByDefault: true));
            }

            try
            {
                if (await SendConfirmEmailAsync(user))
                {
                    return Ok(new ApiResponse(200, title: SM.T_EmailSent, message: SM.M_ConfirmEmailSend));
                }

                return BadRequest(new ApiResponse(400, title: SM.T_EmailSentFailed,
                   message: SM.M_EmailSentFailed, displayByDefault: true));
            }
            catch (Exception)
            {
                return BadRequest(new ApiResponse(400, title: SM.T_EmailSentFailed,
                    message: SM.M_EmailSentFailed, displayByDefault: true));
            }
        }

        [HttpPost("forgot-username-or-password")]
        public async Task<ActionResult<ApiResponse>> ForgotUsernameOrPassword(EmailDto model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // sending a vague response with a fake delay
                PauseResponse();
                return Ok(new ApiResponse(200, title: SM.T_EmailSent, message: SM.M_ForgotUsernamePasswordSent));
            }

            if (!user.IsActive)
            {
                return Unauthorized(new ApiResponse(401, title: SM.T_AccountSuspended, message: SM.M_AccountSuspended,
                    displayByDefault: true));
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest(new ApiResponse(400, title: SM.T_ConfirmEmailFirst, message: SM.M_ConfirmEmailFirst,
                    displayByDefault: true));
            }

            try
            {
                if (await SendForgotUsernameOrPasswordEmail(user))
                {
                    return Ok(new ApiResponse(200, title: SM.T_EmailSent, message: SM.M_ForgotUsernamePasswordSent));
                }

                return BadRequest(new ApiResponse(400, title: SM.T_EmailSentFailed, message: SM.M_EmailSentFailed,
                    displayByDefault: true));
            }
            catch (Exception)
            {
                return BadRequest(new ApiResponse(400, title: SM.T_EmailSentFailed, message: SM.M_EmailSentFailed,
                   displayByDefault: true));
            }
        }

        [HttpPut("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(ResetPasswordDto model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401, title: SM.T_InvallidToken, message: SM.M_InavlidToken,
                    displayByDefault: true));
            }

            if (!user.IsActive)
            {
                return Unauthorized(new ApiResponse(401, title: SM.T_AccountSuspended, message: SM.M_AccountSuspended,
                    displayByDefault: true));
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest(new ApiResponse(400, title: SM.T_ConfirmEmailFirst, message: SM.M_ConfirmEmailFirst,
                    displayByDefault: true));
            }

            var appUserToken = await Context.AppUserTokens
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Name == SD.FUP && x.Value == model.Token);
            if (appUserToken == null || appUserToken.Expires <= DateTime.UtcNow)
            {
                if (appUserToken != null)
                {
                    Context.RemoveRange(appUserToken);
                    await Context.SaveChangesAsync();
                }

                return Unauthorized(new ApiResponse(401, title: SM.T_InvallidToken, message: SM.M_InavlidToken,
                    displayByDefault: true));
            }

            Context.AppUserTokens.Remove(appUserToken);
            await UserManager.RemovePasswordAsync(user);
            await UserManager.AddPasswordAsync(user, model.NewPassword);

            return Ok(new ApiResponse(200, title: SM.T_PasswordRest, message: SM.M_PasswordRest));
        }

        #region Private Methods
        private async Task<bool> SendForgotUsernameOrPasswordEmail(AppUser user)
        {
            var userToken = await Context.AppUserTokens
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Name == SD.FUP);

            var tokenExpiresInMinutes = TokenExpiresInMinutes();

            if (userToken == null)
            {
                var userTokenToAdd = new AppUserToken
                {
                    UserId = user.Id,
                    Name = SD.FUP,
                    Value = SD.GenerateRandomString(),
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpiresInMinutes),
                    LoginProvider = string.Empty
                };

                Context.AppUserTokens.Add(userTokenToAdd);
                userToken = userTokenToAdd;
            }
            else
            {
                userToken.Value = SD.GenerateRandomString();
                userToken.Expires = DateTime.UtcNow.AddMinutes(tokenExpiresInMinutes);
            }

            await Context.SaveChangesAsync();

            using StreamReader streamReader = System.IO.File.OpenText("EmailTemplates/forgot_username_password.html");
            string htmlBody = streamReader.ReadToEnd();

            string messageBody = string.Format(htmlBody, GetClientUrl(), user.Name,
                user.UserName, user.Email, userToken.Value, tokenExpiresInMinutes);
            var emailSend = new EmailSendDto(user.Email, "Forgot username or password", messageBody);

            return await Services.EmailService.SendEmailAsync(emailSend);
        }
        #endregion
    }
}
