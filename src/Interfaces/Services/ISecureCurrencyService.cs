using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;

namespace Tau_CoinDesk_Api.Interfaces.Services
{
    public interface ISecureCurrencyService
    {
        //Task<IEnumerable<object>> GetEncryptedCurrenciesAsync();
        //Task<IEnumerable<object>> GetAllDecryptedAsync();
        Task<CurrencyDto> GetOneDecryptedAsync(Guid id);
        Task<Currency> CreateEncryptedAsync(Currency currency);
        Task<bool> UpdateEncryptedAsync(Guid id, Currency currency);
        //Task<bool> DeleteAsync(Guid id);
    }
}
