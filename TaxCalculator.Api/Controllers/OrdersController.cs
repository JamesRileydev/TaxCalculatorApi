using System;
using System.Collections.Generic;
using System.Text.Json;
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
        private JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ITaxCalculationService TaxCalculationSvc { get; }

        public OrdersController(ITaxCalculationService taxCalculationSvc)
        {
            TaxCalculationSvc = taxCalculationSvc;
        }

        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return Ok("You successfully reached the Orders Controller");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] List<OrderItem> order)
        {
            //TODO - Add validation for incoming orders
            var id = Guid.NewGuid();

            var (result, error) = await TaxCalculationSvc.CalculateTaxAsync(order, id).ConfigureAwait(false);

            if (error is not null)
            {
                return JsonErrorResult(error, id);
            }

            return JsonSuccessResult(result, id);
        }


        [NonAction]
        private IActionResult JsonErrorResult(ServiceError error, Guid id)
        {
            return new JsonResult(new
            {

            })
            {
                SerializerSettings = SerializerOptions
            };
        }

        [NonAction]
        private IActionResult JsonSuccessResult(Receipt result, Guid id)
        {
            return Ok(result);
        }
    }
}
