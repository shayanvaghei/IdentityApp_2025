using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class PlayController : ApiCoreController
    {
        [HttpGet("get-players")]
        public IActionResult GetPlayers()
        {
            return Ok(new { message = "Only authorized users can view players" });
        }
    }
}
