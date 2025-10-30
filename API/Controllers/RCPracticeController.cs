using API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class RCPracticeController : ApiCoreController
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("public");
        }

        #region Roles


        [HttpGet("admin-role")]
        [Authorize(Roles = $"{SD.AdminRole}")]
        public IActionResult AdminRole()
        {
            return Ok("admin role");
        }

        [HttpGet("moderator-role")]
        [Authorize(Roles = $"{SD.ModeratorRole}")]
        public IActionResult ModeratorRole()
        {
            return Ok("moderator role");
        }

        [HttpGet("user-role")]
        [Authorize(Roles = $"{SD.UserRole}")]
        public IActionResult UserRole()
        {
            return Ok("user role");
        }

        [HttpGet("admin-or-moderator-role")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.ModeratorRole}")]
        public IActionResult AdminOrModeratorRole()
        {
            return Ok("admin or moderator role");
        }

        [HttpGet("admin-or-user-role")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.UserRole}")]
        public IActionResult AdminOrUserRole()
        {
            return Ok("admin or user role");
        }
        #endregion

        #region Policy

        [HttpGet("admin-policy")]
        [Authorize(policy: SD.AdminPolicy)]
        public IActionResult AdminPolicy()
        {
            return Ok("admin policy");
        }

        [HttpGet("moderator-policy")]
        [Authorize(policy: SD.ModeratorPolicy)]
        public IActionResult ModeratorPolicy()
        {
            return Ok("moderator policy");
        }

        [HttpGet("user-policy")]
        [Authorize(policy: SD.UserPolicy)]
        public IActionResult UserPolicy()
        {
            return Ok("user policy");
        }


        [HttpGet("admin-or-moderator-policy")]
        [Authorize(policy: SD.AdminOrModeratorPolicy)]
        public IActionResult AdminOrModeratorPolicy()
        {
            return Ok("admin or moderator policy");
        }

        [HttpGet("admin-and-moderator-policy")]
        [Authorize(policy: "AdminAndModeratorPolicy")]
        public IActionResult AdminAndModeratorPolicy()
        {
            return Ok("admin and moderator policy");
        }

        [HttpGet("all-role-policy")]
        [Authorize(policy: SD.AllRolePolicy)]
        public IActionResult AllRolePolicy()
        {
            return Ok("all role policy");
        }

        #endregion

        #region Claim Policy

        [HttpGet("admin-email-policy")]
        [Authorize(policy: SD.AdminEmailPolicy)]
        public IActionResult AdminEmailPolicy()
        {
            return Ok("admin email policy");
        }

        [HttpGet("vip-policy")]
        [Authorize(policy: SD.VIPPolicy)]
        public IActionResult VIPPolicy()
        {
            return Ok("vip policy");
        }
        #endregion
    }
}
