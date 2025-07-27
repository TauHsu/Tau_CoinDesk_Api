using System.Security.Cryptography;
using System.Text;
using Tau_CoinDesk_Api.Interfaces.Security;

namespace Tau_CoinDesk_Api.Services.Security
{
    public class RsaCertificateService : IRsaCertificateStrategy
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;
        public RsaCertificateService(IConfiguration config)
        {
            var keySize = 2048;

            var privateKeyPem = config["RSA:PrivateKey"];
            var publicKeyPem = config["RSA:PublicKey"];

            if (!string.IsNullOrEmpty(privateKeyPem) && !string.IsNullOrEmpty(publicKeyPem))
            {
                _privateKey = RSA.Create();
                _privateKey.ImportFromPem(privateKeyPem);

                _publicKey = RSA.Create();
                _publicKey.ImportFromPem(publicKeyPem);
            }
            else
            {
                // 沒設定就動態產生一對測試用金鑰
                _privateKey = RSA.Create(keySize);
                _publicKey = _privateKey;
            }
        }

        public string SignData(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = _privateKey.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signatureBytes);
        }

        public bool VerifyData(string data, string signature)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);
            return _publicKey.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
