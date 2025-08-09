using API.Data;
using API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCoreController : ControllerBase
    {
        private Context _context;
        private IConfiguration _config;
        private IServiceUnitOfWork _service;
        protected IConfiguration Configuration => _config ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected Context Context => _context ??= HttpContext.RequestServices.GetService<Context>();
        protected IServiceUnitOfWork Services => _service ??= HttpContext.RequestServices.GetService<IServiceUnitOfWork>();

        protected int TokenExpiresInMinutes()
        {
            return int.Parse(Configuration["Email:TokenExpiresInMinutes"]);
        }

        protected string GetClientUrl()
        {
            return Configuration["JWT:ClientUrl"];
        }

        protected void PauseResponse(double sec = 1.3)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(sec));
                return 42;
            });
            t.Wait();
        }
    }
}
