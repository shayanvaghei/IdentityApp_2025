using API.Utility;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class RegisterDto
    {
        private string _userName;
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Username must be at least {2}, and maximum {1} characters")]
        [RegularExpression(SD.UserNameRegex, ErrorMessage = "Username must contain only a-z A-Z 0-9 characters")]
        public string UserName
        {
            get => _userName;
            set => _userName = value.ToLower();
        }

        private string _email;
        [Required]
        [RegularExpression(SD.EmailRegex, ErrorMessage = "Invalid email address")]
        public string Email
        {
            get => _email;
            set => _email = value.ToLower();
        }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]
        public string Password { get; set; }
    }
}
