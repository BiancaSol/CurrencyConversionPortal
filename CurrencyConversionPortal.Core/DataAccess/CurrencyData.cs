namespace CurrencyConversionPortal.Core.DataAccess
{
    using CurrencyConversionPortal.Core.Entities;
    using CurrencyConversionPortal.Core.ExternalServices;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CurrencyData : ICurrencyData
    {
        private readonly IMemoryCache _cache;
        private readonly ICurrencyConversionApiClient _currencyConversionApiClient;
        private const string SupportedCurrenciesKey = "supported-currencies";

        public CurrencyData(IMemoryCache cache, ICurrencyConversionApiClient currencyConversionApiClient)
        {
            _cache = cache;
            _currencyConversionApiClient = currencyConversionApiClient;
        }

        public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
        {
            if (_cache.TryGetValue(SupportedCurrenciesKey, out List<Currency>? cachedList) && cachedList != null)
            {
                return cachedList;
            }

            var supportedCurrencies = await _currencyConversionApiClient.GetAvailableCurrenciesAsync();

            var currencies = supportedCurrencies
                .Select(x => new Currency
                {
                    Code = x.Key,
                    Description = x.Value
                })
                .ToList();

            _cache.Set(SupportedCurrenciesKey, currencies, TimeSpan.FromHours(24));

            return currencies;
        }
    }
}
