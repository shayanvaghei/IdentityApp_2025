using API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCoreController : ControllerBase
    {
        private Context _context;
        private IConfiguration _config;
        protected IConfiguration Configuration => _config ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected Context Context => _context ??= HttpContext.RequestServices.GetService<Context>();
    }
}
