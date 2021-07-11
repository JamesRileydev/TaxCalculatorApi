using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaxCalculator.Api.Data;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Services
{
    // We would want to keep track of our orders, so I have a method in the MockRepository
    // to simulate an insert into some sort of long term storage
    public interface IOrderCreationService
    {
        Task<(Order, IServiceError)> CreateAndInsertOrderAsync(List<OrderItem> order, Guid id);
    }

    public class OrderCreationService : IOrderCreationService
    {
        private ILogger<OrderCreationService> Log { get; }

        private IMockRepository MockRepo { get; }

        public OrderCreationService(ILogger<OrderCreationService> log, IMockRepository mockRepo)
        {
            Log = log;
            MockRepo = mockRepo;
        }

        public async Task<(Order,IServiceError)> CreateAndInsertOrderAsync(List<OrderItem> orderItems, Guid id)
        {
            Log.LogInformation($"[{id}] Method {nameof(CreateAndInsertOrderAsync)} received an order items, will attempt to create order " +
                               "and insert into long term storage.");

            List<OrderItem> combinedItems;
            try
            {
                combinedItems = orderItems.Distinct().ToList().Select(item =>
                {
                    item.Quantity = orderItems.Count(i => i.Equals(item));
                    return item;
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, $"[{id}] Failed to combine distinct items. See exception for details.");
                return (null, new ServiceError
                {
                    Exception = ex,
                    Message = $"An error occurred while combining items for order ID: {id}." +
                              " See exception for more details."
                });
            }

            var order = new Order
            {
                OrderId = id,
                OrderItems = orderItems,
                CombinedItems = combinedItems
            };

            uint result;
            // This method doesn't throw, but in an actual application it could.
            try
            {
                result = await MockRepo.InsertOrderAsync(order).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, $"[{order.OrderId}] An error occurred while inserting the order into long term storage. " +
                                 "See exception for details");
                return (null, new ServiceError
                {
                    Exception = ex,
                    Message = $"An error occurred while inserting the order into long term storage for order ID: {order.OrderId}." +
                              " See exception for more details."
                });
            }

            if (result == default)
            {
                Log.LogError($"[{order.OrderId}] Failed to insert order into long term storage.");
                return (null, new ServiceError
                {
                    Message = $"Failed to insert order with ID: {order.OrderId} into long term storage."
                });
            }

            return (order, null);
        }
    }
}
