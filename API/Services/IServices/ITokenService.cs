using API.DTOs.MyProfile;
using API.Models;
using System.Threading.Tasks;

namespace API.Services.IServices
{
    public interface ITokenService
    {
        Task<string> CreateJWTAsync(AppUser user);
        QrCodeDto GenerateQrCode(string email);
        bool ValidateCode(string secretKey, string code);
        string CreateMfaToken(string userName);
        string GetUserNameFromMfaToken(string mfaToken);
    }
}
