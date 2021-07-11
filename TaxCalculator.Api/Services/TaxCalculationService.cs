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
        public Task<(Receipt, IServiceError)> CalculateTaxAsync(Order order);
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

        // This method may be more appropriately called GenerateReceiptAsync since that is what will be returned.
        // This service would more than likely be two service, one for calculating tax, another for receipt generation.
        public async Task<(Receipt, IServiceError)> CalculateTaxAsync(Order order)
        {
            Log.LogInformation($"[{order.OrderId}] Method {nameof(CalculateTaxAsync)} received an order will attempt to calculate tax.");

            List<OrderItem> combinedItems;
            try
            {
                combinedItems = order.OrderItems.Distinct().ToList().Select(item =>
                {
                    item.Quantity = order.OrderItems.Count(i => i.Equals(item));
                    return item;
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "[{id}] Failed to combine distinct items. See exception for details.", order.OrderId);
                return (null, new ServiceError
                {
                    Exception = ex,
                    Message = $"An error occurred while combining items for order ID: {order.OrderId}." +
                              " See exception for more details."
                });
            }

            decimal basicTaxRate, importTaxRate;
            try
            {
                // This method doesn't throw, but in an actual application it could.
                (basicTaxRate, importTaxRate) = await MockRepository.GetTaxRatesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "[{id}] An error occurred while retrieving tax rates. See exception for details.", order.OrderId);
                return (null, new ServiceError
                {
                    Exception = ex,
                    Message = $"An error occurred while retrieving tax rates for order ID: {order.OrderId}." +
                              " See exception for more details."
                });
            }

            if (basicTaxRate == 0 || importTaxRate == 0)
            {
                Log.LogError("[{id}] One or more values were not returned while retrieving the tax rates. " +
                             "Order will not be processed", order.OrderId);
                return (null, new ServiceError
                {
                    Message = "One or more values were not returned while retrieving the tax rates." +
                              "Order will not be processed"
                });
            }

            order.CombinedItems = combinedItems;

            var (receipt, error) = await CalculateTaxAsync(order, basicTaxRate, importTaxRate).ConfigureAwait(false);

            if (error is not null)
            {
                return (null, error);
            }

            Log.LogInformation($"[{order.OrderId}] Successfully calculated taxes, returning receipt.");

            return (receipt, null);
        }

        private async Task<(Receipt, IServiceError)> CalculateTaxAsync(Order order, decimal basicTaxRate, decimal importTaxRate)
        {
            var receipt = new Receipt { OrderId = order.OrderId };

            foreach (var item in order.CombinedItems)
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

                // This probably wouldn't happen due to the data validation,
                // but it's good have to check anyway.
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
            var basicTax = CalculateTax(item.Price, basicTaxRate);

            item.SalesTaxEach = basicTax;

            return item;
        }

        private OrderItem CalculateBasicSalesAndImportTax(OrderItem item, decimal basicTaxRate, decimal importTaxRate)
        {
            var basicTax = CalculateTax(item.Price, basicTaxRate);
            var importTax = CalculateTax(item.Price, importTaxRate);

            item.SalesTaxEach = basicTax + importTax;

            return item;
        }

        private OrderItem CalculateImportTax(OrderItem item, decimal importTaxRate)
        {
            var importTax = CalculateTax(item.Price, importTaxRate);

            item.SalesTaxEach = importTax;

            return item;
        }

        private decimal CalculateTax(decimal price, decimal taxRate)
        {
            var tax = price * (taxRate / 100);
            return Math.Ceiling(tax * 20) / 20;
        }
    }
}