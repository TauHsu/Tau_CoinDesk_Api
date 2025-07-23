using Microsoft.AspNetCore.Mvc;
using Tau_CoinDesk_Api.Services;
using Tau_CoinDesk_Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Tau_CoinDesk_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController : ControllerBase
    {
        private readonly ICoinDeskService _coinDeskService;
        private readonly AppDbContext _context;

        public RatesController(ICoinDeskService coinDeskService, AppDbContext context)
        {
            _coinDeskService = coinDeskService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRates()
        {
            // 抓 CoinDesk API
            var json = await _coinDeskService.GetRatesAsync();

            // 解析 JSON (時間與匯率資訊)
            var time = json.RootElement.GetProperty("time").GetProperty("updatedISO").GetString();
            var bpi = json.RootElement.GetProperty("bpi");

            // 格式化時間 (轉成 yyyy/MM/dd HH:mm:ss)
            var updateTime = DateTime.Parse(time!).ToString("yyyy/MM/dd HH:mm:ss");

            // 撈 DB 幣別中文名稱
            var currencies = await _context.Currencies.ToListAsync();

            var result = new
            {
                updatedTime = updateTime,
                rates = bpi.EnumerateObject()
                    .Where(currency => currencies.Any(c => c.Code.Equals(currency.Name, StringComparison.OrdinalIgnoreCase)))
                    .Select(currency =>
                    {
                        var code = currency.Name;
                        var rate = currency.Value.GetProperty("rate").GetString();

                        // 找中文名稱，沒有就給空字串
                        var chineseName = currencies
                            .FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                            ?.ChineseName ?? "";

                        return new
                        {
                            code,
                            chineseName,
                            rate
                        };
                    }).ToList()
            };

            return Ok(result);
        }
    }
}