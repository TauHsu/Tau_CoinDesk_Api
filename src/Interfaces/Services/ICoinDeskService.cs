using System.Text.Json;
//using System.Threading.Tasks;

namespace Tau_CoinDesk_Api.Interfaces.Services
{
    public interface ICoinDeskService
    {
        Task<JsonDocument> GetRatesAsync();
    }
}