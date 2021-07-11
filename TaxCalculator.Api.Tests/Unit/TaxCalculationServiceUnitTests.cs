using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TaxCalculator.Api.Data;
using TaxCalculator.Api.Enums;
using TaxCalculator.Api.Models;
using TaxCalculator.Api.Services;
using Xunit;

namespace TaxCalculator.Api.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class TaxCalculationServiceUnitTests
    {
        [Fact]
        public async Task CalculateTaxAsync_ReturnsServiceError_WhenMockRepoThrowsWhileGettingTaxRates()
        {
            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<TaxCalculationService>();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.GetTaxRatesAsync().Throws(new Exception());

            var (result, error) = await sut.CalculateTaxAsync(new Order { OrderItems = new List<OrderItem>() })
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<ServiceError>(error);
            Assert.Contains("An error occurred while retrieving tax rates", error.Message);
        }

        [Fact]
        public async Task CalculateTaxAsync_ReturnsServiceError_WhenMockRepoReturnsZero()
        {
            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<TaxCalculationService>();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.GetTaxRatesAsync().Returns((0, 1));

            var (result, error) = await sut.CalculateTaxAsync(new Order{ OrderItems = new List<OrderItem>() })
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<ServiceError>(error);
            Assert.Contains("One or more values were not returned while retrieving the tax rates", error.Message);
        }

        [Fact]
        public async Task CalculateTaxAsync_ReturnsServiceError_WhenCalculateTaxAsyncReturnsError()
        {
            var order = new Order
            {
                OrderItems = new List<OrderItem>(new List<OrderItem>{new OrderItem
                {
                    Name = "Name",
                    Category = ItemCategories.Unknown,
                    Price = 1.00m
                }}),
                CombinedItems = new List<OrderItem>(new List<OrderItem>{new OrderItem
                {
                    Name = "Name",
                    Category = ItemCategories.Unknown,
                    Price = 1.00m
                }})
            };

            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<TaxCalculationService>();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.GetTaxRatesAsync().Returns((10, 5));

            var (result, error) = await sut.CalculateTaxAsync(order)
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<ServiceError>(error);
            Assert.Contains("Unable to calculate tax for item category", error.Message);
        }

        [Fact]
        public async Task CalculateTaxAsync_ReturnsReceipt_WhenCalculateBasicTaxAsyncSucceeds()
        {
            var order = new Order
            {
                CombinedItems = new List<OrderItem>{new OrderItem
                {
                    Name = "Name",
                    Category = ItemCategories.Basic,
                    Price = 10.00m,
                    Quantity = 1
                }},
            };

            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<TaxCalculationService>();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.GetTaxRatesAsync().Returns((10, 5));

            var (result, error) = await sut.CalculateTaxAsync(order)
                .ConfigureAwait(false);

            Assert.Null(error);
            Assert.NotNull(result);

            Assert.IsType<Receipt>(result);
            Assert.Equal(1.00m, result.Tax);
            Assert.Equal(11.00m, result.Total);
        }

        [Fact]
        public async Task CalculateTaxAsync_ReturnsReceipt_WhenCalculateImportTaxAsyncSucceeds()
        {
            var order = new Order
            {
                CombinedItems = new List<OrderItem>(new List<OrderItem>{new OrderItem
                {
                    Name = "Name",
                    Category = ItemCategories.ExemptImport,
                    Price = 10.00m,
                    Quantity = 1
                    }}),
            };

            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<TaxCalculationService>();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.GetTaxRatesAsync().Returns((10, 5));

            var (result, error) = await sut.CalculateTaxAsync(order)
                .ConfigureAwait(false);

            Assert.Null(error);
            Assert.NotNull(result);

            Assert.IsType<Receipt>(result);
            Assert.Equal(0.50m, result.Tax);
            Assert.Equal(10.50m, result.Total);
        }

        [Fact]
        public async Task CalculateTaxAsync_ReturnsReceipt_WhenCalculateBasicAndImportTaxAsyncSucceeds()
        {
            var order = new Order
            {
                CombinedItems = new List<OrderItem>{new OrderItem
                {
                    Name = "Name",
                    Category = ItemCategories.BasicImport,
                    Price = 10.00m,
                    Quantity = 2
                }},
            };

            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<TaxCalculationService>();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.GetTaxRatesAsync().Returns((10, 5));

            var (result, error) = await sut.CalculateTaxAsync(order)
                .ConfigureAwait(false);

            Assert.Null(error);
            Assert.NotNull(result);

            Assert.IsType<Receipt>(result);
            Assert.Equal(3.0m, result.Tax);
            Assert.Equal(23.00m, result.Total);
        }

        [Fact]
        public async Task CalculateTaxAsync_ReturnsReceipt_WhenItemIsExcempt()
        {
            var order = new Order
            {
                CombinedItems = new List<OrderItem>{new OrderItem
                {
                    Name = "Name",
                    Category = ItemCategories.Exempt,
                    Price = 10.00m,
                    Quantity = 1
                }},
            };

            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<TaxCalculationService>();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.GetTaxRatesAsync().Returns((10, 5));

            var (result, error) = await sut.CalculateTaxAsync(order)
                .ConfigureAwait(false);

            Assert.Null(error);
            Assert.NotNull(result);

            Assert.IsType<Receipt>(result);
            Assert.Single(result.OrderItems);
            Assert.Equal(0m, result.Tax);
            Assert.Equal(10.00m, result.Total);
        }
    }
}
