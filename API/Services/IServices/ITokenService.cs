using API.DTOs.MyProfile;
using API.Models;

namespace API.Services.IServices
{
    public interface ITokenService
    {
        string CreateJWT(AppUser user);
        QrCodeDto GenerateQrCode(string email);
        bool ValidateCode(string secretKey, string code);
        string CreateMfaToken(string userName);
        string GetUserNameFromMfaToken(string mfaToken);
    }
}
