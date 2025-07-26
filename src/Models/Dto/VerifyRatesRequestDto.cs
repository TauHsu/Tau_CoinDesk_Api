namespace Tau_CoinDesk_Api.Models.Dto
{
    public class VerifyRatesRequestDto
    {
        public RatesResponseDto Data { get; set; } = null!;
        public string Signature { get; set; } = string.Empty;
    }
}
