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
        [Range(1, 4, ErrorMessage = "Item must have category from 1 t0 4")]
        public ItemCategories Category { get; set; }

        [Required]
        [Range(0.01, 9999999999999999.99, ErrorMessage = "Invalid Target Price; Max 18 digits")]
        public decimal Price { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal SalesTaxEach { get; set; }

        public decimal AggregateTotalEach =>
            Price + SalesTaxEach;  

        public decimal AggregateTotal =>
            AggregateTotalEach * Quantity;

        public string CreateItemDescription()
        {
            if (Quantity > 1)
            {
                return $"{Name}: {AggregateTotal} ({Quantity} @ {AggregateTotalEach})";
            }

            return $"{Name}: {AggregateTotal}";
        }
    }
}
