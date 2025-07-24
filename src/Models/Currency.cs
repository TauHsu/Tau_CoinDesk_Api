namespace Tau_CoinDesk_Api.Models
{
    public class Currency
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string ChineseName { get; set; } = string.Empty;
    }
}