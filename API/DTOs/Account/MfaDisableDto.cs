using API.Utility;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class MfaDisableDto
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
    }
}
