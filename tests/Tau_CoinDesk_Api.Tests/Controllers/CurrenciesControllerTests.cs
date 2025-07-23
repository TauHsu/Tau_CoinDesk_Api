using Xunit;
using Tau_CoinDesk_Api.Controllers;
using Tau_CoinDesk_Api.Data;
using Tau_CoinDesk_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Tau_CoinDesk_Api.Tests
{
    public class CurrenciesControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CanCreateAndGetCurrency()
        {
            var context = GetDbContext();
            var controller = new CurrenciesController(context);

            // 新增
            var result = await controller.PostCurrency(new Currency { Code = "USD", ChineseName = "美元" });
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var currency = Assert.IsType<Currency>(createdResult.Value);
            Assert.Equal("USD", currency.Code);

            // 查詢
            var getResult = await controller.GetCurrencies();
            var list = Assert.IsAssignableFrom<IEnumerable<Currency>>(getResult.Value);
            Assert.Single(list);
        }
    }
}