using Tau_CoinDesk_Api.Models;

namespace Tau_CoinDesk_Api.Services
{
    public interface ICurrencyService
    {
        Task<IEnumerable<object>> GetCurrenciesAsync();
        Task<Currency?> GetCurrencyAsync(Guid id);
        Task<Currency?> GetByCodeAsync(string code);
        Task<Currency> CreateCurrencyAsync(Currency currency);
        Task<bool> UpdateCurrencyAsync(Guid id, Currency currency);
        Task<bool> DeleteCurrencyAsync(Guid id);
    }
}