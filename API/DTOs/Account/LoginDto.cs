using API.Utility;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class LoginDto
    {
        private string _userName;
        [Required]
        public string UserName
        {
            get => _userName;
            set => _userName = value.ToLower();
        }

        [Required]
        public string Password { get; set; }
    }
}
