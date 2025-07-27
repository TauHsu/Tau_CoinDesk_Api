using Xunit;
using Moq;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Services;
using Tau_CoinDesk_Api.Interfaces.Services;
using Tau_CoinDesk_Api.Interfaces.Encryption;
using Tau_CoinDesk_Api.Exceptions;

namespace Tau_CoinDesk_Api.Tests
{
    public class SecureCurrencyServiceTests
    {
        private readonly Mock<ICurrencyService> _currencyServiceMock;
        private readonly Mock<IAesEncryptionStrategy> _encryptionMock;
        private readonly SecureCurrencyService _service;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;

        public SecureCurrencyServiceTests()
        {
            _currencyServiceMock = new Mock<ICurrencyService>();
            _encryptionMock = new Mock<IAesEncryptionStrategy>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            _localizerMock.Setup(l => l[It.IsAny<string>()])
                          .Returns((string key) => new LocalizedString(key, key));

            _service = new SecureCurrencyService(_currencyServiceMock.Object, _encryptionMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetOneDecryptedAsync_DecryptsFields_WhenEncrypted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var encryptedCurrency = new CurrencyDto
            {
                Id = id,
                Code = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("USD")),
                Name = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("美元"))
            };

            _currencyServiceMock.Setup(s => s.GetOneCurrencyAsync(id))
                .ReturnsAsync(encryptedCurrency);
            _encryptionMock.Setup(e => e.Decrypt(It.IsAny<string>()))
                .Returns<string>(s => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(s)));

            // Act
            var result = await _service.GetOneDecryptedAsync(id);

            // Assert
            Assert.Equal("USD", result.Code);
            Assert.Equal("美元", result.Name);

            _encryptionMock.Verify(e => e.Decrypt(It.IsAny<string>()), Times.Exactly(2));
        }
        [Fact]
        public async Task GetOneDecryptedAsync_ReturnsRawValues_WhenNotEncrypted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var rawCurrency = new CurrencyDto
            {
                Id = id,
                Code = "USD",
                Name = "美元"
            };

            _currencyServiceMock.Setup(s => s.GetOneCurrencyAsync(id))
                .ReturnsAsync(rawCurrency);

            // Act
            var ex = await Assert.ThrowsAsync<AppException>(() => _service.GetOneDecryptedAsync(id));

            // Assert
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("InvalidOrUnencryptedCurrencyCode", ex.Message);
            _encryptionMock.Verify(e => e.Decrypt(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateEncryptedAsync_EncryptsFields_BeforeCallingCurrencyService()
        {
            // Arrange
            var input = new Currency { Id = Guid.NewGuid(), Code = "USD", ChineseName = "美元" };
            _encryptionMock.Setup(e => e.Encrypt("USD")).Returns("encUSD");
            _encryptionMock.Setup(e => e.Encrypt("美元")).Returns("encTWD");

            _currencyServiceMock.Setup(s => s.CreateCurrencyAsync(It.IsAny<Currency>()))
                .ReturnsAsync((Currency c) => c);

            // Act
            var result = await _service.CreateEncryptedAsync(input);

            // Assert
            Assert.Equal("encUSD", result.Code);
            Assert.Equal("encTWD", result.ChineseName);

            _encryptionMock.Verify(e => e.Encrypt("USD"), Times.Once);
            _encryptionMock.Verify(e => e.Encrypt("美元"), Times.Once);
            _currencyServiceMock.Verify(s => s.CreateCurrencyAsync(It.IsAny<Currency>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEncryptedAsync_EncryptsFields_BeforeCallingCurrencyService()
        {
            // Arrange
            var id = Guid.NewGuid();
            var input = new Currency { Id = id, Code = "GBP", ChineseName = "英鎊" };
            _encryptionMock.Setup(e => e.Encrypt("GBP")).Returns("encGBP");
            _encryptionMock.Setup(e => e.Encrypt("英鎊")).Returns("encTWD");

            _currencyServiceMock.Setup(s => s.UpdateCurrencyAsync(id, It.IsAny<Currency>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateEncryptedAsync(id, input);

            // Assert
            Assert.True(result);

            _encryptionMock.Verify(e => e.Encrypt("GBP"), Times.Once);
            _encryptionMock.Verify(e => e.Encrypt("英鎊"), Times.Once);
            _currencyServiceMock.Verify(s => s.UpdateCurrencyAsync(id, It.IsAny<Currency>()), Times.Once);
        }
    }
}
