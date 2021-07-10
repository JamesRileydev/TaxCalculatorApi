using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaxCalculator.Api.Data;
using TaxCalculator.Api.Enums;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Services
{
    public interface ITaxCalculationService
    {
        public Task<(Receipt, ServiceError)> CalculateTaxAsync(List<OrderItem> items, Guid id);
    }

    public class TaxCalculationService : ITaxCalculationService
    {
        private ILogger<TaxCalculationService> Log { get; }

        private IMockRepository MockRepository { get; }

        public TaxCalculationService(IMockRepository mockRepository, ILogger<TaxCalculationService> log)
        {
            MockRepository = mockRepository;
            Log = log;
        }

        public async Task<(Receipt, ServiceError)> CalculateTaxAsync(List<OrderItem> items, Guid id)
        {
            List<OrderItem> combinedItems;

            try
            {
                combinedItems = items.Distinct().ToList().Select(item =>
                {
                    item.Quantity = items.Count(i => i.Equals(item));
                    return item;
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "[{id}] Failed to combine distinct items. See exception for details.", id);
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
                var calculatedItem = item.Category switch
                {
                    _ when item.Category == ItemCategories.ExemptImport =>
                        CalculateImportTax(item),
                    _ when item.Category == ItemCategories.Basic =>
                        CalculateBasicSalesTax(item),
                    _ when item.Category == ItemCategories.BasicImport =>
                        CalculateBasicSalesAndImportTax(item),
                    _ when item.Category == ItemCategories.Exempt =>
                        item,

                    _ => null
                };

                if (calculatedItem is null)
                {
                    return (null, new ServiceError());
                }
                

            }

            await Task.CompletedTask;
            return (receipt, null);
        }

        private OrderItem CalculateBasicSalesTax(OrderItem item)
        {
            return item;
        }

        private OrderItem CalculateBasicSalesAndImportTax(OrderItem item)
        {
            return item;
        }

        private OrderItem CalculateImportTax(OrderItem item)
        {
            return item;
        }
    }
}