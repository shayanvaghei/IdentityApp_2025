using API.Utility;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        private string _email;
        [Required]
        [RegularExpression(SD.EmailRegex, ErrorMessage = "Invalid email address")]
        public string Email
        {
            get => _email;
            set => _email = value.ToLower();
        }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "New password must be at least {2}, and maximum {1} characters")]
        public string NewPassword { get; set; }
    }
}
