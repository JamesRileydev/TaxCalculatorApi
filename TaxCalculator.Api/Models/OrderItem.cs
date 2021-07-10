using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TaxCalculator.Api.Enums;

namespace TaxCalculator.Api.Models
{
    public record OrderItem
    {
        [Required]
        [NotNull]
        public string Name { get; set; }

        [Required]
        [NotNull]
        public ItemCategories Category { get; set; }

        [Required]
        [NotNull]
        public decimal Price { get; set; }

        [Required]
        [NotNull]
        public int Quantity { get; set; }

        public decimal AggregatePrice => 
            Price * Quantity;

        public decimal AggregateTotal =>
            SalesTax + AggregatePrice;

        public decimal SalesTax { get; set; }
    }
}
