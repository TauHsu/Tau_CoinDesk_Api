using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Interfaces.Services;

namespace Tau_CoinDesk_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController : ControllerBase
    {
        private readonly IRatesService _ratesService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RatesController(IRatesService ratesService, IStringLocalizer<SharedResource> localizer)
        {
            _ratesService = ratesService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetRates()
        {
            var result = await _ratesService.GetRatesAsync();
            var response = new ApiResponse<RatesResponseDto>(true, _localizer["GetSuccess"], result);
            return Ok(response);
        }

        [HttpGet("signed")]
        public async Task<IActionResult> GetSignedRates()
        {
            var result = await _ratesService.GetSignedRatesAsync();
            var response = new ApiResponse<RatesSignedResponseDto>(true, _localizer["GetSuccess"], result);
            return Ok(response);
        }

        [HttpPost("verify")]
        public IActionResult VerifyRates([FromBody] VerifyRatesRequestDto request)
        {
            var isValid = _ratesService.VerifyRates(request.Data, request.Signature);
            var response = new ApiResponse<object>(true, _localizer["VerifySuccess"], isValid);
            return Ok(response);
        }
    }
}