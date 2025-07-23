using API.Utility;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(SD.UserId)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
        public static string GetName(this ClaimsPrincipal user)
        {
            return user.FindFirst(SD.Name)?.Value;
        }
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(SD.UserName)?.Value;
        }
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(SD.Email)?.Value;
        }
    }
}
