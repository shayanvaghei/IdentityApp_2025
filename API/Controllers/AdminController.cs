using API.Data;
using API.DTOs;
using API.DTOs.Admin;
using API.DTOs.Pagination;
using API.Extensions;
using API.Models;
using API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize(Roles = $"{SD.AdminRole}")]
    public class AdminController : ApiCoreController
    {
        private readonly RoleManager<AppRole> _roleManager;

        public AdminController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet("app-roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetApplicationRoles()
        {
            return Ok(await _roleManager.Roles.Select(x => x.Name).ToListAsync());
        }

        [HttpGet("get-users")]
        public async Task<ActionResult<PaginatedList<UserViewDto>>> GetUsers([FromQuery] AllUsersParams parameters)
        {
            var userQuery = Context.Users
                .Where(x => x.UserName != SD.SuperAdminUsername && x.Id != User.GetUserId().GetValueOrDefault())
                .Select(x => new UserViewDto
                {
                    UserId = x.Id,
                    Name = x.Name,
                    UserName = x.UserName,
                    Email = x.Email,
                    EmailConfirmed = x.EmailConfirmed,
                    IsLocked = x.LockoutEnd != null && x.LockoutEnd.Value >= DateTime.UtcNow ? true : false,
                    IsActive = x.IsActive,
                    LockoutEnd = x.LockoutEnd.HasValue ? x.LockoutEnd.Value.UtcDateTime : null,
                    CreatedAt = x.CreatedAt,
                    LastActivity = x.LastActivity,
                    CsvRoles = string.Join(", ", x.Roles.Select(r => r.Role.Name.ElementAt(0).ToString().ToUpper()).ToList()),
                }).AsQueryable();

            if (!string.IsNullOrEmpty(parameters.SearchBy))
            {
                userQuery = userQuery.Where(x => x.UserName.ToLower().Contains(parameters.SearchBy) ||
                                    x.Email.ToLower().Contains(parameters.SearchBy) ||
                                    x.Name.ToLower().Contains(parameters.SearchBy));
            }

            if (!string.IsNullOrEmpty(parameters.CsvRoles))
            {
                var roles = parameters.CsvRoles.Split(',');

                var roleIds = await Context.Roles
                     .Where(c => roles.Contains(c.Name))
                     .Select(c => c.Id)
                     .ToListAsync();

                var userIds = await Context.UserRoles
                    .Where(c => roleIds.Contains(c.RoleId))
                    .Select(c => c.UserId)
                    .ToListAsync();

                userQuery = userQuery.Where(u => userIds.Contains(u.UserId));
            }

            if (!string.IsNullOrEmpty(parameters.LockUnlock))
            {
                if (parameters.LockUnlock.Equals(SD.Lock))
                {
                    userQuery = userQuery.Where(u => u.IsLocked);
                }
                else if (parameters.LockUnlock.Equals(SD.Unlock))
                {
                    userQuery = userQuery.Where(u => !u.IsLocked);
                }
            }

            if (!string.IsNullOrEmpty(parameters.Activation))
            {
                if (parameters.Activation.Equals(SD.Active))
                {
                    userQuery = userQuery.Where(u => u.IsActive);
                }
                else if (parameters.Activation.Equals(SD.Inactive))
                {
                    userQuery = userQuery.Where(u => !u.IsActive);
                }
            }

            userQuery = parameters.SortBy switch
            {
                "id_a" => userQuery.OrderBy(u => u.UserId),
                "id_d" => userQuery.OrderByDescending(u => u.UserId),
                "name_a" => userQuery.OrderBy(u => u.Name),
                "name_d" => userQuery.OrderByDescending(u => u.Name),
                "username_a" => userQuery.OrderBy(u => u.UserName),
                "username_d" => userQuery.OrderByDescending(u => u.UserName),
                "email_a" => userQuery.OrderBy(u => u.Email),
                "email_d" => userQuery.OrderByDescending(u => u.Email),
                "created_a" => userQuery.OrderBy(u => u.CreatedAt),
                "created_d" => userQuery.OrderByDescending(u => u.CreatedAt),
                "lastactivity_a" => userQuery.OrderBy(u => u.LastActivity),
                "lastactivity_d" => userQuery.OrderByDescending(u => u.LastActivity),
                _ => userQuery.OrderBy(u => u.Name)
            };

            var users = await PaginatedList<UserViewDto>.CreateAsync(userQuery.AsNoTracking(), parameters.PageNumber, parameters.PageSize);

            foreach (var user in users)
            {
                user.CreatedAt = user.CreatedAt.ToUtcFormat();
                user.LastActivity = user.LastActivity?.ToUtcFormat();
            }

            return Ok(new PaginatedResult<UserViewDto>(users, users.TotalItemsCount, users.PageNumber, users.PageSize, users.TotalPages));
        }

        [HttpPut("lock-unlock-user")]
        public async Task<ActionResult<ApiResponse>> LockUnlockUser(UserActionDto model)
        {
            if (model.UserId == User.GetUserId())
            {
                return BadRequest(new ApiResponse(400, message: SM.M_InvalidSelfAction, displayByDefault: true));
            }

            var user = await GetAppUserAsync(model.UserId);
            if (user == null) return NotFound(new ApiResponse(404));

            if (!user.IsActive) return BadRequest(new ApiResponse(400,
                message: string.Format("User {0} must be activated before performing such an activity.", user.UserName)));

            if (model.Action.Equals(SD.Lock))
            {
                int daysToLock = model.DaysToLock.GetValueOrDefault() == 0 ? SD.DefaultDaysToLock : model.DaysToLock.GetValueOrDefault();

                await UserManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(daysToLock));
                await AdminActionEmailSendAsync(SD.Lock, user, daysToLock);

                return Ok(new ApiResponse(200, title: "Locked", message:
                    string.Format("The user '{0}' has been locked for {1} days.", user.UserName, daysToLock), showWithToastr: true));
            }
            else if (model.Action.Equals(SD.Unlock))
            {
                user.LockoutEnd = null;
                await UserManager.ResetAccessFailedCountAsync(user);
                await Context.SaveChangesAsync();
                await AdminActionEmailSendAsync(SD.Unblock, user);

                return Ok(new ApiResponse(200, title: "Unlocked", message:
                   string.Format("The user '{0}' has been unlocked.", user.UserName), showWithToastr: true));
            }

            return BadRequest(new ApiResponse(400, message: "Invalid action."));
        }

        [HttpPut("active-inactive-user")]
        public async Task<ActionResult<ApiResponse>> ActiveInactiveUser(UserActionDto model)
        {
            if (model.UserId == User.GetUserId())
            {
                return BadRequest(new ApiResponse(400, message: SM.M_InvalidSelfAction, displayByDefault: true));
            }

            var user = await GetAppUserAsync(model.UserId);
            if (user == null) return NotFound(new ApiResponse(404));

            if (model.Action.Equals(SD.Active))
            {
                user.IsActive = true;
                user.LockoutEnd = null;
                await UserManager.ResetAccessFailedCountAsync(user);
                await Context.SaveChangesAsync();
                await AdminActionEmailSendAsync(SD.Active, user);
                return Ok(new ApiResponse(200, title: "Activated", message: string.Format("The user '{0}' has been activated.", user.UserName), showWithToastr: true));
            }
            else if (model.Action.Equals(SD.Inactive))
            {
                user.IsActive = false;
                await Context.SaveChangesAsync();
                await AdminActionEmailSendAsync(SD.Inactive, user);
                return Ok(new ApiResponse(200, title: "Deactivated", message: string.Format("The user '{0}' has been deactivated.", user.UserName), showWithToastr: true));
            }

            return BadRequest(new ApiResponse(400, message: "Invalid action."));
        }

        [HttpPut("email-confirmation-link/{userId}")]
        public async Task<ActionResult<ApiResponse>> EmailConfirmationLink(int userId)
        {
            if (userId == User.GetUserId())
            {
                return BadRequest(new ApiResponse(400, message: SM.M_InvalidSelfAction, displayByDefault: true));
            }

            var user = await GetAppUserAsync(userId);
            if (user == null) return NotFound(new ApiResponse(404));

            if (await SendConfirmEmailAsync(user))
            {
                user.EmailConfirmed = false;
                await Context.SaveChangesAsync();

                return Ok(new ApiResponse(200, title: SM.T_EmailSent,
                    message: string.Format("Email confirmation has been sent for the user: {0}", user.UserName)));
            }

            return BadRequest(new ApiResponse(500));
        }

        [HttpPut("forgot-username-or-password/{userId}")]
        public async Task<ActionResult<ApiResponse>> ForgotUsernameOrPassword(int userId)
        {
            if (userId == User.GetUserId())
            {
                return BadRequest(new ApiResponse(400, message: SM.M_InvalidSelfAction, displayByDefault: true));
            }

            var user = await GetAppUserAsync(userId);
            if (user == null) return NotFound(new ApiResponse(404));

            if (await SendForgotUsernameOrPasswordEmail(user))
            {
                return Ok(new ApiResponse(200, title: SM.T_EmailSent,
                   message: string.Format("A forgot username or password email has been sent to user {0}.", user.UserName)));
            }

            return BadRequest(new ApiResponse(500));
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (userId == User.GetUserId())
            {
                return BadRequest(new ApiResponse(400, message: SM.M_InvalidSelfAction, displayByDefault: true));
            }

            var user = await GetAppUserAsync(userId);
            if (user == null) return NotFound(new ApiResponse(404));

            var result = await UserManager.DeleteAsync(user);

            if (!result.Succeeded) return BadRequest(new ApiResponse(500));

            await AdminActionEmailSendAsync(SD.Delete, user);
            return Ok(new ApiResponse(200, title: "Deleted", 
                message: string.Format("User account {0} has been permanently deleted.", user.UserName), showWithToastr: true));
        }

        [HttpGet("get-user/{userId}")]
        public async Task<ActionResult<UserAddEditDto>> GetUser(int userId)
        {
            if (userId == User.GetUserId())
            {
                return BadRequest(new ApiResponse(400, message: SM.M_InvalidSelfAction, displayByDefault: true));
            }

            var user = await Context.Users
                .Where(x => x.Id == userId && x.UserName.ToLower() != SD.SuperAdminUsername.ToLower())
                .Select(x => new UserAddEditDto
                {
                    UserId = x.Id,
                    Name = x.UserName,
                    Email = x.Email,
                    EmailConfirmed = x.EmailConfirmed,
                    LastActivity = x.LastActivity,
                    IsActive = x.IsActive,
                    Roles = UserManager.GetRolesAsync(x).GetAwaiter().GetResult()
                }).FirstOrDefaultAsync();

            if (user == null) return NotFound();

            user.LastActivity = user.LastActivity?.ToUtcFormat();

            return Ok(user);
        }

        [HttpPost("add-edit-user")]
        public async Task<IActionResult> AddEditUser(UserAddEditDto model)
        {
            AppUser user;
            string message = "";
            int statusCode = 0;

            if (model.UserId == 0)
            {
                // Create
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required");
                    return BadRequest(new ApiResponse(400));
                }

                if (await CheckEmailExistsAsync(model.Email))
                {
                    return BadRequest(new ApiResponse(400,
                        message: $"An account has been registerd with '{model.Email}'. Please try using another email address.",
                        displayByDefault: true));
                }

                if (await CheckNameExistsAsync(model.Name))
                {
                    return BadRequest(new ApiResponse(400,
                       message: $"An account has been registered with '{model.Name}'. Please try using another name (username)",
                       displayByDefault: true));
                }

                user = new AppUser
                {
                    Name = model.Name,
                    UserName = model.Name.ToLower(),
                    Email = model.Email,
                    LockoutEnabled = true,
                    EmailConfirmed = model.EmailConfirmed,
                    IsActive = model.IsActive
                };

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    message = "New user has been created.";
                    statusCode = 201;
                }
                else
                {
                    statusCode = 500;
                }
            }
            else
            {
                // Edit
                if (model.UserId == User.GetUserId())
                {
                    return BadRequest(new ApiResponse(400, message: SM.M_InvalidSelfAction, displayByDefault: true));
                }

                user = await GetAppUserAsync(model.UserId);
                if (user == null) return NotFound(new ApiResponse(404));

                var isEmailChanged = !user.Email.Equals(model.Email);

                if (isEmailChanged && CheckEmailExistsAsync(model.Email).GetAwaiter().GetResult())
                {
                    return BadRequest(new ApiResponse(400,
                        message: $"An account has been registerd with '{model.Email}'. Please try using another email address.",
                        displayByDefault: true));
                }

                if (!user.UserName.ToLower().Equals(model.Name.ToLower()) && CheckNameExistsAsync(model.Name).GetAwaiter().GetResult())
                {
                    return BadRequest(new ApiResponse(400,
                       message: $"An account has been registered with '{model.Name}'. Please try using another name (username)",
                       displayByDefault: true));
                }

                user.Name = model.Name;
                user.UserName = model.Name.ToLower();
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;
                user.IsActive = model.IsActive;

                if (!string.IsNullOrEmpty(model.Password))
                {
                    await UserManager.RemovePasswordAsync(user);
                    await UserManager.AddPasswordAsync(user, model.Password);
                }

                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    message = "The user has been updated.";
                    statusCode = 200;
                }
                else
                {
                    statusCode = 500;
                }
            }

            if (statusCode == 200 || statusCode == 201)
            {
                var userRoles = await UserManager.GetRolesAsync(user);
                if (userRoles.Count() > 0) await UserManager.RemoveFromRolesAsync(user, userRoles);

                if (model.Roles.Any())
                {
                    foreach (var role in model.Roles)
                    {
                        await UserManager.AddToRoleAsync(user, role);
                    }
                }

                if (!user.EmailConfirmed)
                {
                    await SendConfirmEmailAsync(user);
                    message += " A confirmation email has been sent.";
                }

                return Ok(new ApiResponse(statusCode, message: message, showWithToastr: true, data: user.Id));
            }
            else
            {
                return BadRequest(new ApiResponse(statusCode, showWithToastr: true));
            }
        }

        #region Private Methods
        private async Task<AppUser> GetAppUserAsync(int userId)
        {
            var user = await Context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null) return null;

            if (user.UserName.ToLower().Equals(SD.SuperAdminUsername.ToLower()))
            {
                return null;
            }

            return user;
        }
        private async Task<bool> AdminActionEmailSendAsync(string action, AppUser user, int? daysToLock = null)
        {
            string emailTemplate = "";
            string messageBody = "";
            string title = "";

            switch (action)
            {
                case SD.Lock:
                    emailTemplate = "user_lock";
                    title = "Account Locked";
                    break;
                case SD.Unblock:
                    emailTemplate = "user_unlock";
                    title = "Account Unlocked";
                    break;
                case SD.Active:
                    emailTemplate = "user_active";
                    title = "Account Reactivated";
                    break;
                case SD.Inactive:
                    emailTemplate = "user_inactive";
                    title = "Account Suspended";
                    break;
                case SD.Delete:
                    emailTemplate = "user_delete";
                    title = "Account Deleted";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(action)) return false;

            using StreamReader streamReader = System.IO.File.OpenText($"EmailTemplates/{emailTemplate}.html");
            string htmlBody = streamReader.ReadToEnd();

            if (action.Equals(SD.Lock) && daysToLock != null)
            {
                messageBody = string.Format(htmlBody, GetClientUrl(), user.UserName, user.Email, user.Name, daysToLock);
            }
            else
            {
                messageBody = string.Format(htmlBody, GetClientUrl(), user.UserName, user.Email, user.Name);
            }

            var emailSend = new EmailSendDto(user.Email, title, messageBody);
            return await Services.EmailService.SendEmailAsync(emailSend);
        }
        #endregion
    }
}
