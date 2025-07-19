using API.Models;

namespace API.Services.IServices
{
    public interface ITokenService
    {
        string CreateJWT(AppUser user);
    }
}
