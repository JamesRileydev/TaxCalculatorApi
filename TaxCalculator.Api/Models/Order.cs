using System;
using System.Collections.Generic;

namespace TaxCalculator.Api.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public List<OrderItem> CombinedItems { get; set; } = new();
    }
}
