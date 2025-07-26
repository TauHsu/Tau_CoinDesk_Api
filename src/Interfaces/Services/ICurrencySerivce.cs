using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;

namespace Tau_CoinDesk_Api.Interfaces.Services
{
    public interface ICurrencyService
    {
        Task<IEnumerable<object>> GetCurrenciesAsync();
        Task<CurrencyDto> GetOneCurrencyAsync(Guid id);
        Task<Currency?> GetCurrencyAsync(Guid id); // 查詢單筆
        Task<IEnumerable<Currency>> GetAllRawAsync(); // 查詢全部原始資料
        Task<Currency?> GetByCodeAsync(string code);
        Task<Currency> CreateCurrencyAsync(Currency currency);
        Task<bool> UpdateCurrencyAsync(Guid id, Currency currency);
        Task<bool> DeleteCurrencyAsync(Guid id);
    }
}