namespace Tau_CoinDesk_Api.Interfaces.Encryption
{
    public interface IAesEncryptionStrategy
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}