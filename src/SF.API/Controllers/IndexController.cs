using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

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
        public async Task<IActionResult> TestGet()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:6101/orders/");
            if (!response.IsSuccessStatusCode)
                return BadRequest();

            _logger.LogInformation("Yo!");
            return Ok("Yo!");
        }
    }
}