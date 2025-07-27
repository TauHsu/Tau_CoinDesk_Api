namespace Tau_CoinDesk_Api.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}