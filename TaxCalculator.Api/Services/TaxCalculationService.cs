using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Task<(Receipt, IServiceError)> CalculateTaxAsync(List<OrderItem> items, Guid id);
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

        //This method may be more appropriately called GenerateReceiptAsync since that is what will be returned.
        public async Task<(Receipt, IServiceError)> CalculateTaxAsync(List<OrderItem> items, Guid id)
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

            decimal basicTaxRate;
            try
            {
                // This method doesn't throw, but in an actual application it could.
                basicTaxRate = await MockRepository.GetBasicSalesTaxRateAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "[{id}] An error occurred while retrieving the basic tax rate. See exception for details.", id);
                return (null, new ServiceError
                {
                    Exception = ex,
                    Message = $"An error occurred while retrieving the basic tax rate for order ID: {id}." +
                              " See exception for more details."
                });
            }

            if (basicTaxRate is 0)
            {
                Log.LogError("[{id}] A value was not returned while retrieving the Basic Tax rate", id);
                return (null, new ServiceError
                {
                    Message = "A value was not returned while retrieving the Basic Tax rate."
                });
            }

            decimal importTaxRate;
            try
            {
                // This method doesn't throw, but in an actual application it could.
                importTaxRate = await MockRepository.GetImportTaxRateAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "[{id}] An error occurred while retrieving the import tax rate. See exception for details.", id);
                return (null, new ServiceError
                {
                    Exception = ex,
                    Message = $"An error occurred while retrieving the import tax rate for order ID: {id}." +
                              " See exception for more details."
                });
            }

            if (importTaxRate is 0)
            {
                Log.LogError("[{id}] A value was not returned while retrieving the Import Tax rate", id);
                return (null, new ServiceError
                {
                    Message = "A value was not returned while retrieving the Import Tax rate."
                });
            }

            var order = new Order
            {
                OrderId = id,
                OrderItems = combinedItems
            };

            var (receipt, error) = await CalculateTaxAsync(order, basicTaxRate, importTaxRate).ConfigureAwait(false);

            if (error is not null)
            {
                return (null, error);
            }

            return (receipt, null);
        }

        private async Task<(Receipt, IServiceError)> CalculateTaxAsync(Order order, decimal basicTaxRate, decimal importTaxRate)
        {
            //TODO - Move receipt generation to its own method/service
            var receipt = new Receipt();

            foreach (var item in order.OrderItems)
            {
                var calculatedItem = item.Category switch
                {
                    _ when item.Category == ItemCategories.ExemptImport =>
                        CalculateImportTax(item, importTaxRate),
                    _ when item.Category == ItemCategories.Basic =>
                        CalculateBasicSalesTax(item, basicTaxRate),
                    _ when item.Category == ItemCategories.BasicImport =>
                        CalculateBasicSalesAndImportTax(item, basicTaxRate, importTaxRate),
                    _ when item.Category == ItemCategories.Exempt =>
                        item,

                    _ => null
                };

                if (calculatedItem is null)
                {
                    return (null, new ServiceError
                    {
                        Message = $"Unable to calculate tax for item category: {item.Category}"
                    });
                }

                receipt.Tax += calculatedItem.SalesTaxEach * calculatedItem.Quantity;
                receipt.Total += calculatedItem.AggregateTotal;
                receipt.OrderItems.Add(calculatedItem.CreateItemDescription());
            }

            await Task.CompletedTask;
            return (receipt, null);
        }

        private OrderItem CalculateBasicSalesTax(OrderItem item, decimal basicTaxRate)
        {
            var tax = item.Price * (basicTaxRate / 100);
            var roundedTax = Math.Ceiling(tax * 20) / 20;

            item.SalesTaxEach = roundedTax;

            return item;
        }

        private OrderItem CalculateBasicSalesAndImportTax(OrderItem item, decimal basicTaxRate, decimal importTaxRate)
        {
            var basicTax = item.Price * (basicTaxRate / 100);
            var basicRoundedTax = Math.Ceiling(basicTax * 20) / 20;

            var importTax = item.Price * (importTaxRate / 100);
            var importRoundedTax = Math.Ceiling(importTax * 20) / 20;

            item.SalesTaxEach = basicRoundedTax + importRoundedTax;

            return item;
        }

        private OrderItem CalculateImportTax(OrderItem item, decimal importTaxRate)
        {
            var importTax = item.Price * (importTaxRate / 100);
            var importRoundedTax = Math.Ceiling(importTax * 20) / 20;

            item.SalesTaxEach = importRoundedTax;

            return item;
        }
    }
}