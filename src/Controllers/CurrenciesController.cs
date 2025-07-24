using Microsoft.AspNetCore.Mvc;
using Tau_CoinDesk_Api.Models;
using Tau_CoinDesk_Api.Services;

namespace Tau_CoinDesk_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _service;

        public CurrenciesController(ICurrencyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies()
        {
            var result = await _service.GetCurrenciesAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostCurrency(Currency currency)
        {
            var created = await _service.CreateCurrencyAsync(currency);
            return CreatedAtAction(nameof(GetCurrencies), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(Guid id, Currency currency)
        {
            await _service.UpdateCurrencyAsync(id, currency);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(Guid id)
        {
            await _service.DeleteCurrencyAsync(id);
            return NoContent();
        }
    }
}