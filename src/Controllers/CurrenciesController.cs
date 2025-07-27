using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Interfaces.Services;

namespace Tau_CoinDesk_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _service;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CurrenciesController(ICurrencyService service, IStringLocalizer<SharedResource> localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies()
        {
            var result = await _service.GetCurrenciesAsync();
            var response = new ApiResponse<IEnumerable<object>>(true, _localizer["GetSuccess"], result);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrency(Guid id)
        {
            var result = await _service.GetOneCurrencyAsync(id);
            var response = new ApiResponse<CurrencyDto>(true, _localizer["GetSuccess"], result);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> PostCurrency(Currency currency)
        {
            var created = await _service.CreateCurrencyAsync(currency);
            var response = new ApiResponse<Currency>(true, _localizer["CreateSuccess"], created);
            return CreatedAtAction(nameof(GetCurrencies), new { id = created.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(Guid id, Currency currency)
        {
            var updated = await _service.UpdateCurrencyAsync(id, currency);
            var response = new ApiResponse<bool>(true, _localizer["UpdateSuccess"], updated);
            return Accepted(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(Guid id)
        {
            await _service.DeleteCurrencyAsync(id);
            var response = new ApiResponse<object>(true, _localizer["DeleteSuccess"]);
            return Ok(response);
        }
    }
}