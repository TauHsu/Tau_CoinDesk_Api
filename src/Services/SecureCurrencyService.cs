//using Microsoft.Extensions.Localization;
using Tau_CoinDesk_Api.Models.Entities;
using Tau_CoinDesk_Api.Models.Dto;
using Tau_CoinDesk_Api.Interfaces.Services;
using Tau_CoinDesk_Api.Interfaces.Encryption;

namespace Tau_CoinDesk_Api.Services
{
    public class SecureCurrencyService : ISecureCurrencyService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IAesEncryptionStrategy _aesEncryption;

        public SecureCurrencyService(ICurrencyService currencyService, IAesEncryptionStrategy aesEncryption)
        {
            _currencyService = currencyService;
            _aesEncryption = aesEncryption;
        }

        /*
        // 查詢所有（解密後回傳）
        public async Task<IEnumerable<object>> GetAllDecryptedAsync()
        {
            var currencies = await _currencyService.GetAllRawAsync();
            return currencies.Select(c => new CurrencyDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = _localizer[$"{_aesEncryption.Decrypt(c.ChineseName)}"]
            });
        }
        */

        // 查詢單一（解密後回傳）
        public async Task<CurrencyDto> GetOneDecryptedAsync(Guid id)
        {
            var currency = await _currencyService.GetCurrencyAsync(id);
            return new CurrencyDto
            {
                Id = currency!.Id,
                Code = _aesEncryption.Decrypt(currency.Code),
                Name = _aesEncryption.Decrypt(currency.ChineseName)
            };
        }

        // 新增（加密後存）
        public async Task<Currency> CreateEncryptedAsync(Currency currency)
        {
            currency.Code = _aesEncryption.Encrypt(currency.Code);
            currency.ChineseName = _aesEncryption.Encrypt(currency.ChineseName);
            return await _currencyService.CreateCurrencyAsync(currency);
        }

        // 更新（加密後存）
        public async Task<bool> UpdateEncryptedAsync(Guid id, Currency currency)
        {
            currency.Code = _aesEncryption.Encrypt(currency.Code);
            currency.ChineseName = _aesEncryption.Encrypt(currency.ChineseName);
            return await _currencyService.UpdateCurrencyAsync(id, currency);
        }

        /*
        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _currencyService.DeleteCurrencyAsync(id);
        }
        */
    }
}
