using System;

namespace API.DTOs.Admin
{
    public class UserActivityDto
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
