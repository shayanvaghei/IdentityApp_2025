using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Admin
{
    public class UserActionDto
    {
        public int UserId { get; set; }

        private string _action;
        [Required]
        public string Action
        {
            get => _action;
            set => _action = value != null ? value.ToLower() : null;
        }
        public int? DaysToLock { get; set; }
    }
}
