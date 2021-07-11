using System.Threading.Tasks;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Data
{
    //---------------------------------------------------------//
    // The calls to get the tax rates and insert the data would
    // be in separate repository files. I combined them in this
    // exercise for simplicity.
    //
    // Values that can change such as tax rates would be stored
    // in a database or some other common location that would 
    // be maintained by some other entity, such as accounting.
    // What the actual tax rate is is not the concern of
    // the service using it.
    //
    // For the insert I'm assuming we are using a relational database
    // to keep track of orders
    //---------------------------------------------------------//
    public interface IMockRepository
    {
        Task<(int, int)> GetTaxRatesAsync();

        Task<uint> InsertOrderAsync(Order order);
    }
    public class MockRepository : IMockRepository
    {
        public async Task<(int, int)> GetTaxRatesAsync()
        {
            var basicTaxRate = 10;
            var importTaxRate = 5;

            await Task.CompletedTask;
            return (basicTaxRate, importTaxRate);
        }

        public async Task<uint> InsertOrderAsync(Order order)
        {
            // For this I'm assuming the insert was successful
            // If this were an actual insert I might return the
            // table row ID which could correlate to the order number
            // Or I could return the number of rows affected

            await Task.CompletedTask;
            return 1;
        }
    }
}
