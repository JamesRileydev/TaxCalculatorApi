using System.Collections.Generic;

namespace TaxCalculator.Api.Models
{
    public class Receipt
    {
        public List<string> OrderItems { get; set; } = new();

        public decimal Tax;

        public decimal Total { get; set; }
    }
}
