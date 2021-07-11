using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutofacContrib.NSubstitute;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TaxCalculator.Api.Data;
using TaxCalculator.Api.Models;
using TaxCalculator.Api.Services;
using Xunit;

namespace TaxCalculator.Api.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class OrderCreationServiceUnitTests
    {
        [Fact]
        public async Task CreateAndInsertOrderAsync_ReturnsServiceError_WhenLINQThrows()
        {
            var autoSub = new AutoSubstitute();
            var sut = autoSub.Resolve<OrderCreationService>();

            var (result, error) = await sut.CreateAndInsertOrderAsync(null, new Guid())
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<ServiceError>(error);
            Assert.Contains("An error occurred while combining items", error.Message);
        }

        [Fact]
        public async Task CreateAndInsertAsync_ReturnsServiceError_MockRepoThrows()
        {
            var autoSub = new AutoSubstitute();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.InsertOrderAsync(Arg.Any<Order>()).Throws(new Exception());

            var sut = autoSub.Resolve<OrderCreationService>();

            var (result, error) = await sut.CreateAndInsertOrderAsync(new List<OrderItem>(), new Guid())
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<ServiceError>(error);
            Assert.Contains("An error occurred while inserting", error.Message);
        }

        [Fact]
        public async Task CreateAndInsertAsync_ReturnsServiceError_MockRepoReturnsZero()
        {
            var autoSub = new AutoSubstitute();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.InsertOrderAsync(Arg.Any<Order>()).Returns(0);

            var sut = autoSub.Resolve<OrderCreationService>();

            var (result, error) = await sut.CreateAndInsertOrderAsync(new List<OrderItem>(), new Guid())
                .ConfigureAwait(false);

            Assert.Null(result);
            Assert.NotNull(error);
            Assert.IsType<ServiceError>(error);
            Assert.Contains("Failed to insert", error.Message);
        }

        [Fact]
        public async Task CreateAndInsertAsync_ReturnsOrder_WhenSuccessful()
        {
            var autoSub = new AutoSubstitute();

            var mockRepo = autoSub.Resolve<IMockRepository>();
            mockRepo.InsertOrderAsync(Arg.Any<Order>()).Returns(1);

            var sut = autoSub.Resolve<OrderCreationService>();

            var (result, error) = await sut.CreateAndInsertOrderAsync(new List<OrderItem> { new() }, new Guid())
                .ConfigureAwait(false);

            Assert.Null(error);
            Assert.NotNull(result);
            Assert.IsType<Order>(result);
        }
    }
}
