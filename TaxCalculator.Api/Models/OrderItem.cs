using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaxCalculator.Api.Enums;

namespace TaxCalculator.Api.Models
{
    public record OrderItem : IValidatableObject
    {
        public string Name { get; set; }

        public ItemCategories Category { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal SalesTaxEach { get; set; }

        public decimal AggregateTotalEach =>
            Price + SalesTaxEach;  

        public decimal AggregateTotal =>
            AggregateTotalEach * Quantity;

        // Since an actual receipt would contain more information, it would be better to do this with StringBuilder
        // in a ReceiptBuilderService
        public string CreateItemDescription()
        {
            return Quantity > 1 ?
                $"{Name}: {AggregateTotal} ({Quantity} @ {AggregateTotalEach})" :
                $"{Name}: {AggregateTotal}";
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name is null)
            {
                yield return new ValidationResult("Item must have a 'name' key, with a string value.");
            }

            if (Price <= 0)
            {
                yield return new ValidationResult("Item must have a 'price' key," +
                                                  " with a decimal value greater than zero.");
            }

            if (Category is < (ItemCategories)1 or > (ItemCategories)4)
            {
                yield return new ValidationResult("Item must have a 'category' key, with an integer value between 1 and 4.");
            }

            if (Quantity > 1)
            {
                yield return new ValidationResult(
                    "Item 'quantity' is assumed to be 1 and does not be included, " +
                    "add duplicate items for multiples of the same item");
            }
        }
    }
}
