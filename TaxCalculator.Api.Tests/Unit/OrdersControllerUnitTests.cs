﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutofacContrib.NSubstitute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TaxCalculator.Api.Controllers;
using TaxCalculator.Api.Models;
using TaxCalculator.Api.Services;
using Xunit;

namespace TaxCalculator.Api.Tests.Unit
{
    [Trait("Category", "Unit")]
    public class OrdersControllerUnitTests
    {
        [Fact]
        public async Task PlaceOrder_Returns400BadRequest_WhenJsonIsInvalid()
        {
            var autoSub = new AutoSubstitute();
            var controller = autoSub.Resolve<OrdersController>();

            var itemList = new List<OrderItem>();

            var result = await controller.PlaceOrder(itemList);
            
            Assert.NotNull(result);
            var error = Assert.IsType<JsonResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, error.StatusCode);
        }

        [Fact]
        public async Task PlaceOrder_Return500Error_WhenCalculateTaxAsyncFails()
        {
            var autoSub = new AutoSubstitute();
            var taxCalcSvc = autoSub.Resolve<ITaxCalculationService>();
            taxCalcSvc.CalculateTaxAsync(Arg.Any<List<OrderItem>>(), Arg.Any<Guid>())
                .Returns((null, new ServiceError()));

            var controller = autoSub.Resolve<OrdersController>();
            var result = await controller.PlaceOrder(new List<OrderItem>{new()}).ConfigureAwait(false);

            Assert.NotNull(result);
            var error = Assert.IsType<JsonResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, error.StatusCode);
        }

        [Fact]
        public async Task PlaceOrder_Return201Created_WhenCalculateTaxAsyncSucceeds()
        {
            var autoSub = new AutoSubstitute();
            var taxCalcSvc = autoSub.Resolve<ITaxCalculationService>();
            taxCalcSvc.CalculateTaxAsync(Arg.Any<List<OrderItem>>(), Arg.Any<Guid>())
                .Returns((new Receipt(), null));

            var controller = autoSub.Resolve<OrdersController>();
            var result = await controller.PlaceOrder(new List<OrderItem>{new()}).ConfigureAwait(false);

            Assert.NotNull(result);
            var error = Assert.IsType<JsonResult>(result);
            Assert.Equal(StatusCodes.Status201Created, error.StatusCode);
        }
    }
}
