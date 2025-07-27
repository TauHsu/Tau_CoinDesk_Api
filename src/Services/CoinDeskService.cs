using System.Text.Json;
using Tau_CoinDesk_Api.Interfaces.Services;


namespace Tau_CoinDesk_Api.Services
{
    public class CoinDeskService : ICoinDeskService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CoinDeskService> _logger;
        private const string ApiUrl = "https://api.coindesk.com/v1/bpi/currentprice.json";

        public CoinDeskService(HttpClient httpClient, ILogger<CoinDeskService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<JsonDocument> GetRatesAsync()
        {
            try
            {
                // 呼叫 CoinDesk API
                var response = await _httpClient.GetAsync(ApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    // 例如 403 Forbidden 或 429 Too Many Requests
                    _logger.LogWarning("CoinDesk API Error: {StatusCode} ({Code})", response.StatusCode, (int)response.StatusCode);
                    return GetMockData(true);
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonDocument.Parse(content);
            }
            catch (HttpRequestException ex)
            {
                // DNS 無法解析、連線問題
                _logger.LogError(ex, "CoinDesk API Request Failed (可能 DNS 無法解析或 API 停用)");
                return GetMockData(true);
            }
            catch (TaskCanceledException ex)
            {
                // Timeout
                _logger.LogError(ex, "CoinDesk API Timeout");
                return GetMockData(true);
            }
            catch (Exception ex)
            {
                // 其他非預期錯誤
                _logger.LogError(ex, "Unexpected CoinDesk API Error");
                return GetMockData(true);
            }
        }
        
        private JsonDocument GetMockData(bool isError)
        {
            // Mocking data fallback
            var mockJson = new
            {
                isMock = isError,
                time = new
                {
                    updated = "Aug 3, 2022 20:25:00 UTC",
                    updatedISO = "2022-08-03T20:25:00+00:00",
                    updateduk = "Aug 3, 2022 at 21:25 BST"
                },
                disclaimer = "This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org",
                chartName = "Bitcoin",
                bpi = new
                {
                    USD = new
                    {
                        code = "USD",
                        symbol = "$",
                        rate = "23,342.0112",
                        description = "US Dollar",
                        rate_float = 23342.0112
                    },
                    GBP = new
                    {
                        code = "GBP",
                        symbol = "£",
                        rate = "19,504.3978",
                        description = "British Pound Sterling",
                        rate_float = 19504.3978
                    },
                    EUR = new
                    {
                        code = "EUR",
                        symbol = "€",
                        rate = "22,738.5269",
                        description = "Euro",
                        rate_float = 22738.5269
                    }
                }
            };
            string jsonString = JsonSerializer.Serialize(mockJson);
            return JsonDocument.Parse(jsonString);
        }
    }
}
