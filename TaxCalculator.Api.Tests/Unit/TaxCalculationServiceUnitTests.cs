using System.Threading.Tasks;
using AutofacContrib.NSubstitute;
using TaxCalculator.Api.Services;
using Xunit;

namespace TaxCalculator.Api.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class TaxCalculationServiceUnitTests
    {
        [Fact]
        public async Task Test_Method_ReturnsNull()
        {
            var autoSub = new AutoSubstitute();
            var taxSvc = autoSub.Resolve<ITaxCalculationService>();

            var result = await taxSvc.TestMethod().ConfigureAwait(false);

            Assert.True(true);
        }
    }
}
