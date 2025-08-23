using System.ComponentModel.DataAnnotations;

namespace API.DTOs.MyProfile
{
    public class EditProfileBaseDto
    {
        [Required]
        public string CurrentPassword { get; set; }
    }
}
