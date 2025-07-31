using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.Controllers
{
    public class BotController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello from BotController");
        }
    }
}
