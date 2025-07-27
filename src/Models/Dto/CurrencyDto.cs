namespace Tau_CoinDesk_Api.Models.Dto
{
    public class CurrencyDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}