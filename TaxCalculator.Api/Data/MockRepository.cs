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
    // be maintained by some other entity. In a real situation
    // we would probably have a variety of tax rates base on the 
    // customer and their location that would need to be retrieved.
    //
    // For the insert I'm assuming we are using a relational database
    // to keep track of orders
    //---------------------------------------------------------//
    public interface IMockRepository
    {
        Task<int> GetBasicSalesTaxRateAsync();

        Task<int> GetImportTaxRateAsync();

        Task<long> InsertOrderAsync(Order order);
    }
    public class MockRepository : IMockRepository
    {
        public async Task<int> GetBasicSalesTaxRateAsync()
        {
            await Task.CompletedTask;
            return 10;
        }

        public async Task<int> GetImportTaxRateAsync()
        {
            await Task.CompletedTask;
            return 5;
        }

        public async Task<long> InsertOrderAsync(Order order)
        {
            // For this I'm assuming the insert was successful
            // If this were an actual insert I would return the table row ID

            await Task.CompletedTask;
            return 1;
        }
    }
}
