using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutofacContrib.NSubstitute;
using NSubstitute;
using TaxCalculator.Api.Models;
using TaxCalculator.Api.Services;
using Xunit;

namespace TaxCalculator.Api.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class TaxCalculationServiceUnitTests
    {
        [Fact]
        public async Task CalculateTaxAsync_Returns_NullNull()
        {
            var autoSub = new AutoSubstitute();
            var taxSvc = autoSub.Resolve<ITaxCalculationService>();

            var (result, error) = await taxSvc.CalculateTaxAsync(Arg.Any<List<OrderItem>>(), Arg.Any<Guid>())
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.Null(error);
        }
    }
}
