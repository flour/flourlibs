using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SF.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;
        public IndexController(ILogger<IndexController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult TestGet()
        {
            _logger.LogInformation("Yo!");
            return Ok("Yo!");
        }
    }
}