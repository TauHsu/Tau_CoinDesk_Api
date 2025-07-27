using Tau_CoinDesk_Api.Models.Entities;

namespace Tau_CoinDesk_Api.Interfaces.Repositories
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<Currency?> GetByIdAsync(Guid id);
        Task<Currency?> GetByCodeAsync(string code);
        Task AddAsync(Currency currency);
        Task UpdateAsync(Currency currency);
        Task DeleteAsync(Currency currency);
    }
}