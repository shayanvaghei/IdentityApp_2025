using API.Models;
using API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class ServiceUnitOfWork : IServiceUnitOfWork
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        public ServiceUnitOfWork(IConfiguration config,
            UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }
        public IEmailService EmailService => new EmailService(_config);
        public ITokenService TokenService => new TokenService(_config, _userManager);
    }
}
