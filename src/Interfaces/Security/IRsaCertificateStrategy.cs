namespace Tau_CoinDesk_Api.Interfaces.Security
{
    public interface IRsaCertificateStrategy
    {
        string SignData(string data);
        bool VerifyData(string data, string signature);
    }
}