using System.Security.Cryptography;
using System.Text;
using Tau_CoinDesk_Api.Interfaces.Encryption;

namespace Tau_CoinDesk_Api.Services.Encryption
{
    public class AesEncryptionStrategy : IAesEncryptionStrategy
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptionStrategy(IConfiguration config)
        {
            var keyStr = config["Encryption:AES:Key"];
            var ivStr = config["Encryption:AES:IV"];

            if (string.IsNullOrEmpty(keyStr) || string.IsNullOrEmpty(ivStr))
                throw new InvalidOperationException("AES Key/IV not configured in appsettings.json");

            _key = Encoding.UTF8.GetBytes(keyStr);
            _iv = Encoding.UTF8.GetBytes(ivStr);
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(cipher);
        }

        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plain = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plain);
        }
    }
}
