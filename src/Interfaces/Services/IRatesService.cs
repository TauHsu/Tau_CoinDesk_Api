using Tau_CoinDesk_Api.Models.Dto;

namespace Tau_CoinDesk_Api.Interfaces.Services
{
    public interface IRatesService
    {
        Task<RatesResponseDto> GetRatesAsync();
        Task<RatesSignedResponseDto> GetSignedRatesAsync();
        bool VerifyRates(RatesResponseDto Data, string Signature);
    }
}