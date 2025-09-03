using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class MfaVerifyDto
    {
        [Required]
        public string MfaToken { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
