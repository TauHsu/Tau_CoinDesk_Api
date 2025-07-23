using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tau_CoinDesk_Api.Data;
using Tau_CoinDesk_Api.Models;

namespace Tau_CoinDesk_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CurrenciesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            return await _context.Currencies.OrderBy(c => c.Code).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Currency>> PostCurrency(Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCurrencies), new { id = currency.Id }, currency);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(int id, Currency currency)
        {
            if (id != currency.Id) return BadRequest();
            _context.Entry(currency).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null) return NotFound();

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}