using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TaxCalculator.Api.Services
{
    public interface ITaxCalculationService
    {
        public Task<IActionResult> TestMethod();
    }

    public class TaxCalculationService : ITaxCalculationService
    {
        public async Task<IActionResult> TestMethod()
        {
            await Task.CompletedTask;
            return null;
        }
    }
}