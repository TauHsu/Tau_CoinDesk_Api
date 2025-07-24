using Xunit;
using Tau_CoinDesk_Api.Controllers;
using Tau_CoinDesk_Api.Models;
using Tau_CoinDesk_Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tau_CoinDesk_Api.Exceptions;

namespace Tau_CoinDesk_Api.Tests
{
    public class CurrenciesControllerTests
    {
        private readonly Guid _testGuid1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private readonly Guid _testGuid2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        [Fact]
        public async Task GetCurrencies_ReturnsList()
        {
            var mockService = new Mock<ICurrencyService>();
            mockService.Setup(s => s.GetCurrenciesAsync())
                       .ReturnsAsync(new List<Currency>
                       {
                           new Currency { Id = _testGuid1, Code = "USD", ChineseName = "美元" },
                           new Currency { Id = _testGuid2, Code = "GBP", ChineseName = "英鎊" }
                       });

            var controller = new CurrenciesController(mockService.Object);

            var result = await controller.GetCurrencies();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<Currency>>(okResult.Value);
            Assert.Equal(2, ((List<Currency>)list).Count);
        }

        [Fact]
        public async Task PostCurrency_ReturnsCreated()
        {
            var mockService = new Mock<ICurrencyService>();
            var createdCurrency = new Currency { Id = _testGuid1, Code = "USD", ChineseName = "美元" };
            mockService.Setup(s => s.CreateCurrencyAsync(It.IsAny<Currency>()))
                       .ReturnsAsync(createdCurrency);

            var controller = new CurrenciesController(mockService.Object);

            var result = await controller.PostCurrency(new Currency { Code = "USD", ChineseName = "美元" });
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var currency = Assert.IsType<Currency>(createdResult.Value);
            Assert.Equal("USD", currency.Code);
        }

        [Fact]
        public async Task UpdateCurrency_ReturnsNoContent_WhenSuccessful()
        {
            var mockService = new Mock<ICurrencyService>();
            mockService.Setup(s => s.UpdateCurrencyAsync(_testGuid1, It.IsAny<Currency>()))
                       .ReturnsAsync(true);

            var controller = new CurrenciesController(mockService.Object);

            var result = await controller.UpdateCurrency(_testGuid1, new Currency { Code = "USD" });
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateCurrency_ReturnsNotFound_WhenFailed()
        {
            var mockService = new Mock<ICurrencyService>();
            mockService.Setup(s => s.UpdateCurrencyAsync(_testGuid1, It.IsAny<Currency>()))
                    .ThrowsAsync(new AppException(404, "Currency not found"));

            var controller = new CurrenciesController(mockService.Object);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                controller.UpdateCurrency(_testGuid1, new Currency { Code = "USD" })
            );

            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("Currency not found", ex.Message);
        }

        [Fact]
        public async Task DeleteCurrency_ReturnsNoContent_WhenSuccessful()
        {
            var mockService = new Mock<ICurrencyService>();
            mockService.Setup(s => s.DeleteCurrencyAsync(_testGuid1))
                    .ReturnsAsync(true);

            var controller = new CurrenciesController(mockService.Object);

            var result = await controller.DeleteCurrency(_testGuid1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCurrency_ReturnsNotFound_WhenFailed()
        {
            var mockService = new Mock<ICurrencyService>();
            mockService.Setup(s => s.DeleteCurrencyAsync(_testGuid1))
                    .ThrowsAsync(new AppException(404, "Currency not found"));

            var controller = new CurrenciesController(mockService.Object);

            var ex = await Assert.ThrowsAsync<AppException>(() => controller.DeleteCurrency(_testGuid1));
            
            Assert.Equal(404, ex.StatusCode);
        }
    }
}
