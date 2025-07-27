using Microsoft.EntityFrameworkCore;
using Tau_CoinDesk_Api.Data;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Interfaces.Repositories;

namespace Tau_CoinDesk_Api.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;

        public CurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return await _context.Currencies.OrderBy(c => c.Code).ToListAsync();
        }

        public async Task<Currency?> GetByIdAsync(Guid id)
        {
            return await _context.Currencies.FindAsync(id);
        }

        public async Task<Currency?> GetByCodeAsync(string code)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task AddAsync(Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Currency currency)
        {
            _context.Entry(currency).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Currency currency)
        {
            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
        }
    }
}