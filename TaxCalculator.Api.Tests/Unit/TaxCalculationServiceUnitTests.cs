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
        public async Task CalculateTaxAsync_ReturnsServiceError_WhenLINQThrows()
        {
            var autoSub = new AutoSubstitute();
            var taxSvc = autoSub.Resolve<TaxCalculationService>();

            var (result, error) = await taxSvc.CalculateTaxAsync(null, Guid.NewGuid())
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<ServiceError>(error);
            Assert.Contains("An error occurred while combining items", error.Message);
        }
    }
}
