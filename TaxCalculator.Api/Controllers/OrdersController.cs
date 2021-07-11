using System;
using System.Collections.Generic;
using System.Net;
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
        private JsonSerializerOptions SerializerOptions { get; } = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public ITaxCalculationService TaxCalculationSvc { get; }

        public OrdersController(ITaxCalculationService taxCalculationSvc)
        {
            TaxCalculationSvc = taxCalculationSvc;
        }

        // This is here for testing purposes.
        // You can navigate to http://localhost:9090/api/orders to see the message
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return Ok("You successfully reached the Orders Controller");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] List<OrderItem> orderItems)
        {
            if (!ModelState.IsValid || orderItems.Count == 0)
            {
                return ErrorJsonResult(HttpStatusCode.BadRequest,
                    new ServiceError
                    {
                        Message = "Payload must contain at least one valid item in a JSON array."
                    });
            }

            var id = Guid.NewGuid();

            var order = new Order
            {
                OrderId = id,
                OrderItems = orderItems
            };

            var (result, error) = await TaxCalculationSvc.CalculateTaxAsync(order).ConfigureAwait(false);

            if (error is not null)
            {
                return ErrorJsonResult(HttpStatusCode.InternalServerError, error);
            }

            return CreatedJsonResult(result, id);
        }


        [NonAction]
        private IActionResult ErrorJsonResult(HttpStatusCode status , IServiceError error)
        {
            return new JsonResult(error, SerializerOptions)
            {
                StatusCode = (int)status
            };
        }
        
        [NonAction]
        private IActionResult CreatedJsonResult(Receipt result, Guid id)
        {
            return new JsonResult(new
            {
                id,
                result.OrderItems,
                result.Tax,
                result.Total
            }, SerializerOptions)
            {
                StatusCode = (int)HttpStatusCode.Created
            };
        }
    }
}
