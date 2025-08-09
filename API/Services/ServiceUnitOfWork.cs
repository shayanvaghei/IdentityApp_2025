using API.Services.IServices;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class ServiceUnitOfWork : IServiceUnitOfWork
    {
        private readonly IConfiguration _config;

        public ServiceUnitOfWork(IConfiguration config)
        {
            _config = config;
        }
        public IEmailService EmailService => new EmailService(_config);
        public ITokenService TokenService => new TokenService(_config);
    }
}
