using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Services;
using Tau_CoinDesk_Api.Interfaces.Services;
using Tau_CoinDesk_Api.Interfaces.Encryption;

namespace Tau_CoinDesk_Api.Tests
{
    public class SecureCurrencyServiceTests
    {
        private readonly Mock<ICurrencyService> _currencyServiceMock;
        private readonly Mock<IAesEncryptionStrategy> _encryptionMock;
        private readonly SecureCurrencyService _service;

        public SecureCurrencyServiceTests()
        {
            _currencyServiceMock = new Mock<ICurrencyService>();
            _encryptionMock = new Mock<IAesEncryptionStrategy>();
            _service = new SecureCurrencyService(_currencyServiceMock.Object, _encryptionMock.Object);
        }

        [Fact]
        public async Task GetOneDecryptedAsync_CallsDecrypt_OnFields()
        {
            // Arrange
            var id = Guid.NewGuid();
            var encryptedCurrency = new Currency
            {
                Id = id,
                Code = "encUSD",
                ChineseName = "encTWD"
            };

            _currencyServiceMock.Setup(s => s.GetCurrencyAsync(id))
                .ReturnsAsync(encryptedCurrency);
            _encryptionMock.Setup(e => e.Decrypt("encUSD")).Returns("USD");
            _encryptionMock.Setup(e => e.Decrypt("encTWD")).Returns("美元");

            // Act
            var result = await _service.GetOneDecryptedAsync(id);

            // Assert
            Assert.Equal("USD", result.Code);
            Assert.Equal("美元", result.Name);

            _currencyServiceMock.Verify(s => s.GetCurrencyAsync(id), Times.Once);
            _encryptionMock.Verify(e => e.Decrypt("encUSD"), Times.Once);
            _encryptionMock.Verify(e => e.Decrypt("encTWD"), Times.Once);
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
