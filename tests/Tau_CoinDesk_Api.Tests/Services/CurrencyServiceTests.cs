using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Services;
using Tau_CoinDesk_Api.Interfaces.Repositories;
using Tau_CoinDesk_Api.Exceptions;

namespace Tau_CoinDesk_Api.Tests.Services
{
    public class CurrencyServiceTests
    {
        private readonly Mock<ICurrencyRepository> _repoMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly CurrencyService _service;

        public CurrencyServiceTests()
        {
            _repoMock = new Mock<ICurrencyRepository>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            // localizer 預設直接回傳 key
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                          .Returns((string key) => new LocalizedString(key, key));

            _service = new CurrencyService(_repoMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetCurrencyAsync_ShouldReturnCurrency_WhenFound()
        {
            var id = Guid.NewGuid();
            var currency = new Currency { Id = id, Code = "USD", ChineseName = "美元" };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(currency);

            var result = await _service.GetCurrencyAsync(id);

            Assert.NotNull(result);
            Assert.Equal("USD", result?.Code);
        }

        [Fact]
        public async Task GetCurrencyAsync_ShouldReturnNull_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Currency?)null);

            var result = await _service.GetCurrencyAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllRawAsync_ShouldReturnCurrencies()
        {
            var currencies = new List<Currency>
            {
                new Currency { Code = "USD", ChineseName = "美元" },
                new Currency { Code = "JPY", ChineseName = "日圓" }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(currencies);

            var result = await _service.GetAllRawAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnCurrency_WhenFound()
        {
            var currency = new Currency { Code = "USD", ChineseName = "美元" };
            _repoMock.Setup(r => r.GetByCodeAsync("USD")).ReturnsAsync(currency);

            var result = await _service.GetByCodeAsync("USD");

            Assert.NotNull(result);
            Assert.Equal("USD", result?.Code);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByCodeAsync("USD")).ReturnsAsync((Currency?)null);

            var result = await _service.GetByCodeAsync("USD");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOneCurrencyAsync_ShouldReturnCurrencyDto_WhenFound()
        {
            var id = Guid.NewGuid();
            var currency = new Currency { Id = id, Code = "USD", ChineseName = "美元" };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(currency);

            var result = await _service.GetOneCurrencyAsync(id);

            Assert.Equal(id, result.Id);
            Assert.Equal("USD", result.Code);
            Assert.Equal("USD", result.Name);
        }

        [Fact]
        public async Task GetOneCurrencyAsync_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Currency?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => _service.GetOneCurrencyAsync(id));
            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("CurrencyNotFound", ex.Message);
        }

        [Fact]
        public async Task CreateCurrencyAsync_ShouldCreate_WhenCodeNotExists()
        {
            var currency = new Currency { Code = "USD", ChineseName = "美元" };
            _repoMock.Setup(r => r.GetByCodeAsync("USD")).ReturnsAsync((Currency?)null);

            var result = await _service.CreateCurrencyAsync(currency);

            Assert.Equal("USD", result.Code);
            Assert.NotEqual(Guid.Empty, result.Id);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Currency>()), Times.Once);
        }

        [Fact]
        public async Task CreateCurrencyAsync_ShouldThrow_WhenCodeExists()
        {
            var currency = new Currency { Code = "USD" };
            _repoMock.Setup(r => r.GetByCodeAsync("USD")).ReturnsAsync(new Currency { Code = "USD" });

            var ex = await Assert.ThrowsAsync<AppException>(() => _service.CreateCurrencyAsync(currency));
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("CurrencyCodeExists", ex.Message);
        }

        [Fact]
        public async Task UpdateCurrencyAsync_ShouldUpdate_WhenValid()
        {
            var id = Guid.NewGuid();
            var existCurrency = new Currency { Id = id, Code = "JPY", ChineseName = "日圓" };
            var newCurrency = new Currency { Code = "USD", ChineseName = "美元" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existCurrency);

            var result = await _service.UpdateCurrencyAsync(id, newCurrency);

            Assert.True(result);
            Assert.Equal("USD", existCurrency.Code);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Currency>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCurrencyAsync_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Currency?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => _service.UpdateCurrencyAsync(id, new Currency()));
            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("CurrencyNotFound", ex.Message);
        }

        [Fact]
        public async Task UpdateCurrencyAsync_ShouldThrow400_WhenDataNotChanged()
        {
            var existCurrency = new Currency { Id = Guid.NewGuid(), Code = "USD", ChineseName = "美元" };
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existCurrency);

            var newCurrency = new Currency { Code = "USD", ChineseName = "美元" };


            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _service.UpdateCurrencyAsync(existCurrency.Id, newCurrency)
            );
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("CurrencyDataNotChange", ex.Message);
        }

        [Fact]
        public async Task UpdateCurrencyAsync_ShouldThrow400_WhenUniqueConstraintViolation()
        {
            var existing = new Currency { Id = Guid.NewGuid(), Code = "USD", ChineseName = "美元" };
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existing);

            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Currency>()))
                .ThrowsAsync(new DbUpdateException("UNIQUE constraint failed", new Exception("UNIQUE constraint failed")));

            var input = new Currency { Code = "EUR", ChineseName = "歐元" };

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _service.UpdateCurrencyAsync(existing.Id, input)
            );

            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("CurrencyCodeExists", ex.Message);
        }

        [Fact]
        public async Task DeleteCurrencyAsync_ShouldDelete_WhenFound()
        {
            var id = Guid.NewGuid();
            var existCurrency = new Currency { Id = id, Code = "USD" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existCurrency);

            var result = await _service.DeleteCurrencyAsync(id);

            Assert.True(result);
            _repoMock.Verify(r => r.DeleteAsync(existCurrency), Times.Once);
        }

        [Fact]
        public async Task DeleteCurrencyAsync_ShouldThrow_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Currency?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => _service.DeleteCurrencyAsync(id));
            Assert.Equal(404, ex.StatusCode);
            Assert.Equal("CurrencyNotFound", ex.Message);
        }
    }
}
