using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaxCalculator.Api.Models;
using TaxCalculator.Api.Services;

namespace TaxCalculator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        public ITaxCalculationService TaxCalculationSvc { get; }

        public OrdersController(ITaxCalculationService taxCalculationSvc)
        {
            TaxCalculationSvc = taxCalculationSvc;
        }

        public async Task<IActionResult> Index()
        {
            var result = await TaxCalculationSvc.TestMethod().ConfigureAwait(false);

            return Ok("You successfully reached the Orders Controller");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] List<OrderItem> items)
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
