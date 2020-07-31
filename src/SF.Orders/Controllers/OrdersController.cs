using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SF.Orders.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpGet("single")]
        public async Task<IActionResult> GetOrder()
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            _logger.LogInformation("Got orders");
            return Ok(Enumerable.Range(1, 20).Select(i => new { Order = i, Name = $"Order #{i}" }));
        }
    }
}