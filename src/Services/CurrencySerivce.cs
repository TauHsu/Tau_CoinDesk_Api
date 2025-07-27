using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Interfaces.Repositories;
using Tau_CoinDesk_Api.Interfaces.Services;
using Tau_CoinDesk_Api.Exceptions;

namespace Tau_CoinDesk_Api.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepo;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CurrencyService(ICurrencyRepository currencyRepo, IStringLocalizer<SharedResource> localizer)
        {
            _currencyRepo = currencyRepo;
            _localizer = localizer;
        }

        public async Task<IEnumerable<object>> GetCurrenciesAsync()
        {
            var currencies = await _currencyRepo.GetAllAsync();
            return currencies.Select(c => new
            {
                c.Id,
                c.Code,
                Name = _localizer[$"{c.Code}"]
            });
        }

        public async Task<Currency?> GetCurrencyAsync(Guid id)
        {
            return await _currencyRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Currency>> GetAllRawAsync()
        {
            return await _currencyRepo.GetAllAsync();
        }

        public async Task<Currency?> GetByCodeAsync(string code)
        {
            return await _currencyRepo.GetByCodeAsync(code);
        }

        public async Task<CurrencyDto> GetOneCurrencyAsync(Guid id)
        {
            var currency = await _currencyRepo.GetByIdAsync(id);
            if (currency == null)
            {
                throw new AppException(404, _localizer["CurrencyNotFound"]);
            }

            return new CurrencyDto
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = _localizer[$"{currency.Code}"]
            };
        }

        public async Task<Currency> CreateCurrencyAsync(Currency currency)
        {
            var exist = await _currencyRepo.GetByCodeAsync(currency.Code);
            if (exist != null)
            {
                throw new AppException(400, _localizer["CurrencyCodeExists"]);
            }

            currency.Id = Guid.NewGuid();
            await _currencyRepo.AddAsync(currency);
            return currency;
        }

        public async Task<bool> UpdateCurrencyAsync(Guid id, Currency currency)
        {
            var existCurrency = await _currencyRepo.GetByIdAsync(id);
            if (existCurrency == null)
            {
                throw new AppException(404, _localizer["CurrencyNotFound"]);
            }
            if (currency.Code == existCurrency.Code && currency.ChineseName == existCurrency.ChineseName)
            {
                throw new AppException(400, _localizer["CurrencyDataNotChange"]);
            }

            existCurrency.Code = currency.Code;
            existCurrency.ChineseName = currency.ChineseName;

            try
            {
                await _currencyRepo.UpdateAsync(existCurrency);
                return true;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true)
                {
                    throw new AppException(400, _localizer["CurrencyCodeExists"]);
                }

                throw;
            }
        }

        public async Task<bool> DeleteCurrencyAsync(Guid id)
        {
            var currency = await _currencyRepo.GetByIdAsync(id);
            if (currency == null)
            {
                throw new AppException(404, _localizer["CurrencyNotFound"]);
            }

            await _currencyRepo.DeleteAsync(currency);
            return true;
        }
    }
}