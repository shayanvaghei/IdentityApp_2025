using System;

namespace API.DTOs.Admin
{
    public class UserViewDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivity { get; set; }
        public string CsvRoles { get; set; }
    }
}
