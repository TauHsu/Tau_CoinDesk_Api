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
    public class RatesControllerTests
    {
        private readonly Mock<IRatesService> _ratesServiceMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly RatesController _controller;

        public RatesControllerTests()
        {
            _ratesServiceMock = new Mock<IRatesService>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            // 預設 Localizer 回傳 "Success"
            _localizerMock
                .Setup(l => l["GetSuccess"])
                .Returns(new LocalizedString("GetSuccess", "Success"));
            _localizerMock
                .Setup(l => l["VerifySuccess"])
                .Returns(new LocalizedString("VerifySuccess", "Verified"));

            _controller = new RatesController(_ratesServiceMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetRates_ReturnsOk_WithApiResponse()
        {
            // Arrange
            var fakeResult = new RatesResponseDto(); // 可以依需求初始化
            _ratesServiceMock.Setup(s => s.GetRatesAsync()).ReturnsAsync(fakeResult);

            // Act
            var result = await _controller.GetRates();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<RatesResponseDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(fakeResult, apiResponse.Data);
        }

        [Fact]
        public async Task GetSignedRates_ReturnsOk_WithApiResponse()
        {
            // Arrange
            var fakeResult = new RatesSignedResponseDto(); // 可初始化
            _ratesServiceMock.Setup(s => s.GetSignedRatesAsync()).ReturnsAsync(fakeResult);

            // Act
            var result = await _controller.GetSignedRates();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<RatesSignedResponseDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(fakeResult, apiResponse.Data);
        }

        [Fact]
        public void VerifyRates_ReturnsOk_WithApiResponse()
        {
            // Arrange
            var fakeRates = new RatesResponseDto
            {
                UpdatedTime = "2025/07/26 12:00:00",
                Rates = new List<RateItemDto>
                {
                    new RateItemDto { Code = "USD", Name = "US Dollar", Rate = "23,342.0112" }
                }
            };
            var request = new VerifyRatesRequestDto { Data = fakeRates, Signature = "sig" };
            _ratesServiceMock.Setup(s => s.VerifyRates(fakeRates, "sig")).Returns(true);

            // Act
            var result = _controller.VerifyRates(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.True((bool)apiResponse.Data!);
        }
    }
}
