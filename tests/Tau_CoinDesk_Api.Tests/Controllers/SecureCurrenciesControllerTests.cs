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
    public class SecureCurrenciesControllerTests
    {
        private readonly Mock<ISecureCurrencyService> _secureServiceMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly SecureCurrenciesController _controller;

        public SecureCurrenciesControllerTests()
        {
            _secureServiceMock = new Mock<ISecureCurrencyService>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            // 預設 Localizer 回傳 key 文字
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                          .Returns((string key) => new LocalizedString(key, key));

            _controller = new SecureCurrenciesController(_secureServiceMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetOneDecrypted_ReturnsOk_WithCurrencyDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedCurrency = new CurrencyDto { Id = id, Code = "USD", Name = "美元" };
            _secureServiceMock
                .Setup(s => s.GetOneDecryptedAsync(id))
                .ReturnsAsync(expectedCurrency);

            // Act
            var result = await _controller.GetOneDecrypted(id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var response = Assert.IsType<ApiResponse<CurrencyDto>>(result.Value);
            Assert.True(response.Success);
            Assert.Equal("GetSuccess", response.Message);
            Assert.Equal(expectedCurrency, response.Data);

            _secureServiceMock.Verify(s => s.GetOneDecryptedAsync(id), Times.Once);
        }

        [Fact]
        public async Task PostCurrency_ReturnsCreated_WithCurrency()
        {
            // Arrange
            var newCurrency = new Currency { Id = Guid.NewGuid(), Code = "EUR", ChineseName = "歐元" };
            _secureServiceMock
                .Setup(s => s.CreateEncryptedAsync(newCurrency))
                .ReturnsAsync(newCurrency);

            // Act
            var result = await _controller.PostCurrency(newCurrency) as CreatedResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal($"/api/currencies/{newCurrency.Id}", result.Location);

            var response = Assert.IsType<ApiResponse<Currency>>(result.Value);
            Assert.True(response.Success);
            Assert.Equal("CreateSuccess", response.Message);
            Assert.Equal(newCurrency, response.Data);

            _secureServiceMock.Verify(s => s.CreateEncryptedAsync(newCurrency), Times.Once);
        }

        [Fact]
        public async Task UpdateCurrency_ReturnsAccepted_WithBool()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateCurrency = new Currency { Id = id, Code = "GBP", ChineseName = "英鎊" };

            _secureServiceMock
                .Setup(s => s.UpdateEncryptedAsync(id, updateCurrency))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateCurrency(id, updateCurrency) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(202, result.StatusCode);

            var response = Assert.IsType<ApiResponse<bool>>(result.Value);
            Assert.True(response.Success);
            Assert.Equal("UpdateSuccess", response.Message);
            Assert.True(response.Data);

            _secureServiceMock.Verify(s => s.UpdateEncryptedAsync(id, updateCurrency), Times.Once);
        }
    }
}
