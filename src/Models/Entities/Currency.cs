using Microsoft.EntityFrameworkCore;

namespace Tau_CoinDesk_Api.Models.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Currency
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string ChineseName { get; set; } = string.Empty;
    }
}