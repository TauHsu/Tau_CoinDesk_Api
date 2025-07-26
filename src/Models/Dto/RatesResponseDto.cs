namespace Tau_CoinDesk_Api.Models.Dto
{
    public class RatesResponseDto
    {
        public string UpdatedTime { get; set; } = string.Empty;
        public List<RateItemDto> Rates { get; set; } = new List<RateItemDto>();
    }

    public class RatesSignedResponseDto
    {
        public RatesResponseDto Data { get; set; } = null!;
        public string Signature { get; set; } = string.Empty;
    }

    public class RateItemDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Rate { get; set; } = string.Empty;
    }
}