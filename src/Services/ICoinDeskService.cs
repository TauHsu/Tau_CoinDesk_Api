using System.Text.Json;
using System.Threading.Tasks;

namespace Tau_CoinDesk_Api.Services
{
    public interface ICoinDeskService
    {
        Task<JsonDocument> GetRatesAsync();
    }
}