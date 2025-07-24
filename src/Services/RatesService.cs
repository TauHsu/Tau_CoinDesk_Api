using Microsoft.Extensions.Localization;
//using System.Text.Json;
//using Tau_CoinDesk_Api.Models;
//using Tau_CoinDesk_Api.Services;
using Tau_CoinDesk_Api.Repositories;

namespace Tau_CoinDesk_Api.Services
{
    public class RatesService : IRatesService
    {
        private readonly ICoinDeskService _coinDeskService;
        private readonly ICurrencyRepository _currencyRepo;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RatesService(ICoinDeskService coinDeskService,
                            ICurrencyRepository currencyRepo,
                            IStringLocalizer<SharedResource> localizer)
        {
            _coinDeskService = coinDeskService;
            _currencyRepo = currencyRepo;
            _localizer = localizer;
        }

        public async Task<RatesResponseDto> GetRatesAsync()
        {
            // 抓 CoinDesk API
            var json = await _coinDeskService.GetRatesAsync();

            // 解析 JSON (時間與匯率資訊)
            var time = json.RootElement.GetProperty("time").GetProperty("updatedISO").GetString();
            var bpi = json.RootElement.GetProperty("bpi");

            // 格式化時間 (轉成 yyyy/MM/dd HH:mm:ss)
            var updateTime = DateTime.Parse(time!).ToString("yyyy/MM/dd HH:mm:ss");

            // 撈 DB 幣別中文名稱
            var currencies = await _currencyRepo.GetAllAsync();

            var rates = bpi.EnumerateObject()
                .Where(currency => currencies.Any(c => c.Code.Equals(currency.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(currency =>
                {
                    var code = currency.Name;
                    var rate = currency.Value.GetProperty("rate").GetString();
                    var localizedName = _localizer[code];

                    /*
                    // 找中文名稱，沒有就給空字串
                    var chineseName = currencies
                        .FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                        ?.ChineseName ?? "";
                    */
                    return new RateItemDto
                    {
                        Code = code,
                        Name = localizedName,
                        Rate = rate ?? ""
                    };
                }).ToList();

            return new RatesResponseDto
            {
                UpdatedTime = updateTime,
                Rates = rates
            };
        }
    }
}