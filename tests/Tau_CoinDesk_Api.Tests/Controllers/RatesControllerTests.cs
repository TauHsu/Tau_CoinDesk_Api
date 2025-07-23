using Xunit;
using Tau_CoinDesk_Api.Controllers;
using Tau_CoinDesk_Api.Data;
using Tau_CoinDesk_Api.Models;
using Tau_CoinDesk_Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;

namespace Tau_CoinDesk_Api.Tests
{
    public class RatesControllerTests
    {
        private readonly AppDbContext _context;

        public RatesControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "RatesTestDb") // 使用記憶體資料庫
                .Options;

            _context = new AppDbContext(options);

            // 確保資料只加一次
            if (!_context.Currencies.Any())
            {
                _context.Currencies.AddRange(
                    new Currency { Code = "USD", ChineseName = "美元" },
                    new Currency { Code = "GBP", ChineseName = "英鎊" }
                );
                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task ReturnsRatesWithChineseNames()
        {
            var mockService = new Mock<ICoinDeskService>();
            var mockJson = JsonDocument.Parse(@"{
                ""time"": { ""updatedISO"": ""2022-08-03T20:25:00+00:00"" },
                ""bpi"": {
                    ""USD"": { ""rate"": ""23,342.0112"" },
                    ""GBP"": { ""rate"": ""19,504.3978"" }
                }
            }");
            mockService.Setup(s => s.GetRatesAsync()).ReturnsAsync(mockJson);

            var controller = new RatesController(mockService.Object, _context);

            var result = await controller.GetRates();
            var ok = Assert.IsType<OkObjectResult>(result);

            var json = JsonSerializer.Serialize(ok.Value);
            using var doc = JsonDocument.Parse(json);

            // 檢查時間格式
            string updatedTime = doc.RootElement.GetProperty("updatedTime").GetString();
            Assert.Equal("2022/08/04 04:25:00", updatedTime);

            // 直接取出 rates 陣列
            var rates = doc.RootElement.GetProperty("rates").EnumerateArray().ToList();
            Assert.Equal(2, rates.Count);

            // 驗證是否有包含 美元 和 英鎊
            Assert.Contains(rates, r => r.GetProperty("code").GetString() == "USD" &&
                                        r.GetProperty("chineseName").GetString() == "美元");
            Assert.Contains(rates, r => r.GetProperty("code").GetString() == "GBP" &&
                                        r.GetProperty("chineseName").GetString() == "英鎊");
        }
    }
}