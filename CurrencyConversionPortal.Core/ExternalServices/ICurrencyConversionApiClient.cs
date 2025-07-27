namespace CurrencyConversionPortal.Core.ExternalServices
{
    using CurrencyConversionPortal.Core.Models.Api;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface ICurrencyConversionApiClient
    {
        Task<CurrencyRatesResponse> GetConversionRatesAsync(string sourceCurrency, List<string> targetCurrencies);

        Task<Dictionary<string, string>> GetAvailableCurrenciesAsync();
    }
}