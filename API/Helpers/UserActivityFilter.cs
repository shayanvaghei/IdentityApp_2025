using API.Data;
using API.DTOs.Admin;
using API.Extensions;
using API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class UserActivityFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                await next();
                return;
            }

            var userId = context.HttpContext.User.GetUserId();
            if (userId == null || userId == 0) return;

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<Context>();

            var user = await dbContext.Users
                .Where(x => x.Id == userId)
                .Select(x => new UserActivityDto
                {
                    UserId = x.Id,
                    IsActive = x.IsActive,
                    LockoutEnd = x.LockoutEnd
                }).FirstOrDefaultAsync();

            if (user == null || !user.IsActive || (user.LockoutEnd != null && user.LockoutEnd.Value.DateTime > DateTime.UtcNow))
            {
                context.HttpContext.Response.Cookies.Delete(SD.IdentityAppCookie);

                context.Result = new ObjectResult(403)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // ExecuteSqlInterpolatedAsync is safe from SQL injection
            await dbContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE [AspNetUsers] SET [LastActivity] = {DateTime.UtcNow} WHERE [Id] = {userId}");

            await next();
        }
    }
}
