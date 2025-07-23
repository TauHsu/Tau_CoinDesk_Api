using Microsoft.EntityFrameworkCore;
using Tau_CoinDesk_Api.Models;

namespace Tau_CoinDesk_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Currency> Currencies { get; set; }
    }
}