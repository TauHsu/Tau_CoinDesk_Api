namespace Tau_CoinDesk_Api.Services
{
    public interface IRatesService
    {
        Task<RatesResponseDto> GetRatesAsync();
    }
}