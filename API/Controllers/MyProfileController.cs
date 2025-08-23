using API.DTOs;
using API.DTOs.MyProfile;
using API.Extensions;
using API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class MyProfileController : ApiCoreController
    {
        [HttpGet]
        public async Task<ActionResult<MyProfileDto>> GetMyProfile()
        {
            var userProfile = await Context.Users
                .Where(x => x.Id == User.GetUserId())
                .Select(x => new MyProfileDto
                {
                    Name = x.Name,
                    Email = x.Email,
                }).FirstOrDefaultAsync();

            if (userProfile == null) return NotFound();
            return userProfile;
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse>> EditMyProfile(EditMyProfileDto model)
        {
            var user = await UserManager.FindByNameAsync(User.GetUserName());
            if (user == null) return NotFound();

            var message = await UserPasswordValidationAsync(user, model.CurrentPassword);
            if (!string.IsNullOrEmpty(message))
            {
                return Unauthorized(new ApiResponse(401, message: message, displayByDefault: true, isHtmlEnabled: true));
            }

            var isEmailChaned = !user.Email.Equals(model.Email);
            if (isEmailChaned && await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest(new ApiResponse(400,
                    message: $"An account has been registerd with '{model.Email}'. Please try using another email address.",
                    displayByDefault: true));
            }

            if (!user.UserName.Equals(model.Name.ToLower()) && await CheckNameExistsAsync(model.Name))
            {
                return BadRequest(new ApiResponse(400,
                    message: $"An account has been registered with '{model.Name}'. Please try using another name (username)",
                    displayByDefault: true));
            }

            user.Name = model.Name;
            user.UserName = model.Name.ToLower();
            user.NormalizedUserName = model.Name.ToUpper();

            if (isEmailChaned)
            {
                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
                user.EmailConfirmed = false;

                try
                {
                    if (await SendConfirmEmailAsync(user))
                    {
                        RemoveJwtCookie();
                        return Ok(new ApiResponse(200,
                            title: "Email was changed",
                            message: "Your changes have been saved, and you've been logged out due to the email change. Please confirm your email address."));
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
            else
            {
                await Context.SaveChangesAsync();
                var appUserDto = CreateAppUserDto(user);

                return Ok(new ApiResponse(200, message: "Your changes have been saved successfully.",
                    showWithToastr: true, displayByDefault: true, data: appUserDto));
            }
        }

        [HttpPut("change-password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword(ChangePasswordDto model)
        {
            var user = await UserManager.FindByNameAsync(User.GetUserName());
            if (user == null) return NotFound();

            var message = await UserPasswordValidationAsync(user, model.CurrentPassword);
            if (!string.IsNullOrEmpty(message))
            {
                return Unauthorized(new ApiResponse(401, message: message, displayByDefault: true, isHtmlEnabled: true));
            }

            var result = await UserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return Ok(new ApiResponse(200, message: "Your password has been changed successfully.", showWithToastr: true));
        }

        [HttpDelete("delete-account")]
        public async Task<ActionResult<ApiResponse>> DeleteAccount(DeleteAccountDto model)
        {
            var user = await UserManager.FindByNameAsync(User.GetUserName());
            if (user == null) return NotFound();

            if (!model.Confirmation)
            {
                return BadRequest(new ApiResponse(400, message: "Please accept confirmation.", displayByDefault: true));
            }

            if (!user.UserName.Equals(model.CurrentUserName))
            {
                return BadRequest(new ApiResponse(400, message: "Invalid username. Please try again.", displayByDefault: true));
            }

            var message = await UserPasswordValidationAsync(user, model.CurrentPassword);
            if (!string.IsNullOrEmpty(message))
            {
                return Unauthorized(new ApiResponse(401, message: message, displayByDefault: true, isHtmlEnabled: true));
            }

            var result = await UserManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(500));

            RemoveJwtCookie();
            return Ok(new ApiResponse(200, message: "Your user account has been permanently deleted.", displayByDefault: true));
        }
    }
}
