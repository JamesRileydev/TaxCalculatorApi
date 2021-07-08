using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Services
{
    public interface ITaxCalculationService
    {
        public Task<(Receipt, ServiceError)> CalculateTaxAsync(List<OrderItem> items, Guid id);
    }

    public class TaxCalculationService : ITaxCalculationService
    {
        public async Task<(Receipt, ServiceError)> CalculateTaxAsync(List<OrderItem> items, Guid id)
        {
            IEnumerable combinedItems;

            try
            {
                combinedItems = items.Distinct().ToList().Select(item =>
                {
                    item.Quantity = items.Count(i => i.Equals(item));
                    return item;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (null, new ServiceError
                {
                    Exception = ex,
                    Message = $"An error occurred while combining items for order ID: {id}." +
                              " See exception for more details."
                });
            }

            var receipt = new Receipt();

            foreach (var item in combinedItems)
            {
                
            }

            await Task.CompletedTask;
            return (receipt, null);
        }
    }
}