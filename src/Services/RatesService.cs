using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Interfaces.Repositories;
using Tau_CoinDesk_Api.Interfaces.Services;
using Tau_CoinDesk_Api.Interfaces.Security;
using Tau_CoinDesk_Api.Exceptions;
using System.Text.Json;

namespace Tau_CoinDesk_Api.Services
{
    public class RatesService : IRatesService
    {
        private readonly ICoinDeskService _coinDeskService;
        private readonly ICurrencyRepository _currencyRepo;
        private readonly IRsaCertificateStrategy _rsa;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly JsonSerializerOptions _jsonOptions;

        public RatesService(ICoinDeskService coinDeskService,
                            ICurrencyRepository currencyRepo,
                            IRsaCertificateStrategy rsa,
                            IStringLocalizer<SharedResource> localizer)
        {
            _coinDeskService = coinDeskService;
            _currencyRepo = currencyRepo;
            _rsa = rsa;
            _localizer = localizer;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
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

        public async Task<RatesSignedResponseDto> GetSignedRatesAsync()
        {
            var result = await GetRatesAsync();
            var jsonString = JsonSerializer.Serialize(result);
            var signature = _rsa.SignData(jsonString);

            return new RatesSignedResponseDto
            {
                Data = result,
                Signature = signature
            };
        }
        
        public bool VerifyRates(RatesResponseDto Data, string Signature)
        {
            var jsonString = JsonSerializer.Serialize(Data);
            var isValid = _rsa.VerifyData(jsonString, Signature);
            if (!isValid)
            {
                throw new AppException(400, _localizer["VerifyFail"]);
            }

            return isValid;
        }
    }
}