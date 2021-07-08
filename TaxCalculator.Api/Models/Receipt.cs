using System.Collections.Generic;

namespace TaxCalculator.Api.Models
{
    public class Receipt
    {
        public List<OrderItem> OrderItems { get; set; } = new();

        public decimal Tax;

        public decimal Total { get; set; }
    }
}
