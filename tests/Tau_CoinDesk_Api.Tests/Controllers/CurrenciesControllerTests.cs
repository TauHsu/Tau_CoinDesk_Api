using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Controllers;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Interfaces.Services;

namespace Tau_CoinDesk_Api.Tests
{
    public class CurrenciesControllerTests
    {
        private readonly Mock<ICurrencyService> _serviceMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly CurrenciesController _controller;

        public CurrenciesControllerTests()
        {
            _serviceMock = new Mock<ICurrencyService>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            // 預設 localizer 回傳 key 本身
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                          .Returns((string key) => new LocalizedString(key, key));

            _controller = new CurrenciesController(_serviceMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetCurrencies_ReturnsOkResult_WithApiResponse()
        {
            // Arrange
            var fakeCurrencies = new List<object> { new { Code = "USD", Name = "美元" } };
            _serviceMock.Setup(s => s.GetCurrenciesAsync())
                        .ReturnsAsync(fakeCurrencies);

            // Act
            var result = await _controller.GetCurrencies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<object>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(fakeCurrencies, response.Data);
        }

        [Fact]
        public async Task GetCurrency_ReturnsOkResult_WithApiResponse()
        {
            // Arrange
            var fakeCurrency = new CurrencyDto { Id = Guid.NewGuid(), Code = "USD", Name = "美元" };
            _serviceMock.Setup(s => s.GetOneCurrencyAsync(fakeCurrency.Id))
                        .ReturnsAsync(fakeCurrency);

            // Act
            var result = await _controller.GetCurrency(fakeCurrency.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CurrencyDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(fakeCurrency, response.Data);
        }

        [Fact]
        public async Task PostCurrency_ReturnsCreatedAtAction_WithApiResponse()
        {
            // Arrange
            var newCurrency = new Currency { Id = Guid.NewGuid(), Code = "USD", ChineseName = "美元" };
            _serviceMock.Setup(s => s.CreateCurrencyAsync(It.IsAny<Currency>()))
                        .ReturnsAsync(newCurrency);

            // Act
            var result = await _controller.PostCurrency(newCurrency);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<Currency>>(createdResult.Value);
            Assert.True(response.Success);
            Assert.Equal(newCurrency, response.Data);
        }

        [Fact]
        public async Task UpdateCurrency_ReturnsAccepted_WithApiResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.UpdateCurrencyAsync(id, It.IsAny<Currency>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateCurrency(id, new Currency());

            // Assert
            var acceptedResult = Assert.IsType<AcceptedResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(acceptedResult.Value);
            Assert.True(response.Success);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task DeleteCurrency_ReturnsOkResult_WithApiResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteCurrencyAsync(id))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCurrency(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
        }
    }
}
