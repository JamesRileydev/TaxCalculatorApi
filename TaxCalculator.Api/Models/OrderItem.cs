using System;
using TaxCalculator.Api.Enums;

namespace TaxCalculator.Api.Models
{
    public class OrderItem
    {
        public string Name { get; set; }

        public ItemCategories Category { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
