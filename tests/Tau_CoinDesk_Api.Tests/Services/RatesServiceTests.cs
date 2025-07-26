using Xunit;
using Moq;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Services;
using Tau_CoinDesk_Api.Interfaces.Repositories;
using Tau_CoinDesk_Api.Interfaces.Services;
using Tau_CoinDesk_Api.Interfaces.Security;
using Tau_CoinDesk_Api.Exceptions;
using System.Text.Json;

namespace Tau_CoinDesk_Api.Tests.Services
{
    public class RatesServiceTests
    {
        private readonly Mock<ICoinDeskService> _coinDeskMock;
        private readonly Mock<ICurrencyRepository> _currencyRepoMock;
        private readonly Mock<IRsaCertificateStrategy> _rsaMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly RatesService _service;

        public RatesServiceTests()
        {
            _coinDeskMock = new Mock<ICoinDeskService>();
            _currencyRepoMock = new Mock<ICurrencyRepository>();
            _rsaMock = new Mock<IRsaCertificateStrategy>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();
            
            // localizer 預設直接回傳 key
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                          .Returns((string key) => new LocalizedString(key, key));

            _service = new RatesService(
                _coinDeskMock.Object,
                _currencyRepoMock.Object,
                _rsaMock.Object,
                _localizerMock.Object);
        }

        [Fact]
        public async Task GetRatesAsync_ReturnsRatesResponseDto()
        {
            // Arrange
            var fakeJson = JsonDocument.Parse(@"{
            ""time"": { ""updatedISO"": ""2025-07-26T12:00:00Z"" },
            ""bpi"": {
                ""USD"": { ""rate"": ""31.50"" }
            }
        }");

            var fakeCurrencies = new List<Currency>
        {
            new Currency { Code = "USD", ChineseName = "美元" }
        };

            _coinDeskMock.Setup(c => c.GetRatesAsync()).ReturnsAsync(fakeJson);
            _currencyRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeCurrencies);

            // Act
            var result = await _service.GetRatesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2025/07/26 20:00:00", result.UpdatedTime); // 依照時區會調整，測試時請注意
            Assert.Single(result.Rates);
            Assert.Equal("USD", result.Rates[0].Code);
            Assert.Equal("USD", result.Rates[0].Name); // 因為 Localizer 直接回 key
            Assert.Equal("31.50", result.Rates[0].Rate);
        }

        [Fact]
        public async Task GetSignedRatesAsync_ReturnsSignedResponse()
        {
            // Arrange
            var fakeRates = new RatesResponseDto
            {
                UpdatedTime = "2025/07/26 12:00:00",
                Rates = new List<RateItemDto> { new RateItemDto { Code = "USD", Name = "美元", Rate = "31.50" } }
            };

            var fakeJson = JsonDocument.Parse(@"{
            ""time"": { ""updatedISO"": ""2025-07-26T12:00:00Z"" },
            ""bpi"": {
                ""USD"": { ""rate"": ""31.50"" }
            }
        }");

            _coinDeskMock.Setup(c => c.GetRatesAsync()).ReturnsAsync(fakeJson);
            _currencyRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Currency> { new Currency { Code = "USD" } });
            _rsaMock.Setup(r => r.SignData(It.IsAny<string>())).Returns("signature123");

            // Act
            var result = await _service.GetSignedRatesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("signature123", result.Signature);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public void VerifyRates_ReturnsTrue_WhenSignatureValid()
        {
            // Arrange
            var rates = new RatesResponseDto { UpdatedTime = "2025/07/26", Rates = new List<RateItemDto>() };
            _rsaMock.Setup(r => r.VerifyData(It.IsAny<string>(), "sig")).Returns(true);

            // Act
            var result = _service.VerifyRates(rates, "sig");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyRates_Throws_WhenSignatureInvalid()
        {
            // Arrange
            var rates = new RatesResponseDto { UpdatedTime = "2025/07/26", Rates = new List<RateItemDto>() };
            _rsaMock.Setup(r => r.VerifyData(It.IsAny<string>(), "sig")).Returns(false);
            _localizerMock.Setup(l => l["VerifyFail"]).Returns(new LocalizedString("VerifyFail", "驗證失敗"));

            // Act & Assert
            var ex = Assert.Throws<AppException>(() => _service.VerifyRates(rates, "sig"));
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("驗證失敗", ex.Message);
        }
    }
}
