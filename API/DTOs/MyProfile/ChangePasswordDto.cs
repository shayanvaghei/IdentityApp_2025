using System.ComponentModel.DataAnnotations;

namespace API.DTOs.MyProfile
{
    public class ChangePasswordDto : EditProfileBaseDto
    {
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "New password must be at least {2}, and maximum {1} characters")]
        public string NewPassword { get; set; }
    }
}
