using Xunit;
using Tau_CoinDesk_Api.Controllers;
using Tau_CoinDesk_Api.Data;
using Tau_CoinDesk_Api.Models;
using Tau_CoinDesk_Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tau_CoinDesk_Api.Tests
{
    public class RatesControllerTests
    {
        [Fact]
        public async Task ReturnsRatesWithChineseNames()
        {
            // 模擬 RatesService
            var mockRatesService = new Mock<IRatesService>();

            var mockResult = new RatesResponseDto
            {
                UpdatedTime = "2022/08/04 04:25:00",
                Rates = new List<RateItemDto>
                {
                    new RateItemDto { Code = "USD", Name = "美元", Rate = "23,342.0112" },
                    new RateItemDto { Code = "GBP", Name = "英鎊", Rate = "19,504.3978" }
                }
            };

            mockRatesService
                .Setup(s => s.GetRatesAsync())
                .ReturnsAsync(mockResult);

            var controller = new RatesController(mockRatesService.Object);

            var actionResult = await controller.GetRates();

            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            var returnValue = Assert.IsType<RatesResponseDto>(okResult.Value);

            Assert.Equal("2022/08/04 04:25:00", returnValue.UpdatedTime);
            Assert.Equal(2, returnValue.Rates.Count);

            Assert.Contains(returnValue.Rates, r => r.Code == "USD" && r.Name == "美元");
            Assert.Contains(returnValue.Rates, r => r.Code == "GBP" && r.Name == "英鎊");

        }
    }
}