using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Interfaces.Services;

namespace Tau_CoinDesk_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureCurrenciesController : ControllerBase
    {
        private readonly ISecureCurrencyService _secureService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SecureCurrenciesController(ISecureCurrencyService secureService, IStringLocalizer<SharedResource> localizer)
        {
            _secureService = secureService;
            _localizer = localizer;
        }

        /*
        [HttpGet]
        public async Task<IActionResult> GetDecrypted()
        {
            var decrypted = await _secureService.GetAllDecryptedAsync();
            var response = new ApiResponse<IEnumerable<object>>(true, _localizer["GetSuccess"], decrypted);
            return Ok(response);
        }
        */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneDecrypted(Guid id)
        {
            var decrypted = await _secureService.GetOneDecryptedAsync(id);
            var response = new ApiResponse<CurrencyDto>(true, _localizer["GetSuccess"], decrypted);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> PostCurrency([FromBody] Currency currency)
        {
            var created = await _secureService.CreateEncryptedAsync(currency);
            var location = $"/api/currencies/{created.Id}";
            var response = new ApiResponse<Currency>(true, _localizer["CreateSuccess"], created);
            return Created(location, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(Guid id, [FromBody]  Currency currency)
        {
            var updated = await _secureService.UpdateEncryptedAsync(id, currency);
            var response = new ApiResponse<bool>(true, _localizer["UpdateSuccess"], updated);
            return Accepted(response);
        }

        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(Guid id)
        {
            await _secureService.DeleteAsync(id);
            var response = new ApiResponse<object>(true, _localizer["DeleteSuccess"]);
            return Ok(response);
        }
        */
    }
}
