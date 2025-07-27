namespace CurrencyConversionPortal.Core.Services
{
    using CurrencyConversionPortal.Core.ExternalServices;
    using CurrencyConversionPortal.Core.DataAccess;
    using CurrencyConversionPortal.Core.Entities;
    using CurrencyConversionPortal.Core.Models;
    using CurrencyConversionPortal.Core.Models.Api;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyData _currencyData;
        private readonly ICurrencyConversionApiClient _currencyConversionApiClient;

        public CurrencyService(ICurrencyData currencyData, ICurrencyConversionApiClient currencyConversionApiClient)
        {
            _currencyData = currencyData;
            _currencyConversionApiClient = currencyConversionApiClient;
        }

        public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
        {
            return await _currencyData.GetCurrenciesAsync();
        }

        public async Task<ConversionModel> ConvertAsync(decimal amount, string sourceCurrency)
        {
            ValidateConversionRequest(amount, sourceCurrency);

            try
            {
                var allCurrencies = await GetCurrencyCodesAsync();
                ValidateCurrencyCode(sourceCurrency, allCurrencies);

                var targetCurrencies = GetTargetCurrencies(sourceCurrency, allCurrencies);
                var apiResponse = await _currencyConversionApiClient.GetConversionRatesAsync(sourceCurrency, targetCurrencies);
                
                return BuildConversionResult(amount, sourceCurrency, apiResponse);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Currency conversion API request failed: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse currency conversion API response: {ex.Message}", ex);
            }
        }

        private void ValidateConversionRequest(decimal amount, string sourceCurrency)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));
            }

            if (string.IsNullOrWhiteSpace(sourceCurrency))
            {
                throw new ArgumentException("Source currency cannot be empty", nameof(sourceCurrency));
            }
        }

        private async Task<List<string>> GetCurrencyCodesAsync()
        {
            return (await _currencyData.GetCurrenciesAsync()).Select(c => c.Code).ToList();
        }

        private void ValidateCurrencyCode(string currencyCode, List<string> availableCurrencies)
        {
            if (!availableCurrencies.Contains(currencyCode, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid source currency: {currencyCode}", nameof(currencyCode));
            }
        }

        private List<string> GetTargetCurrencies(string sourceCurrency, List<string> allCurrencies)
        {
            return allCurrencies
                .Where(c => !c.Equals(sourceCurrency, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private ConversionModel BuildConversionResult(decimal amount, string sourceCurrency, CurrencyRatesResponse apiResponse)
        {
            var results = apiResponse.Rates.ToDictionary(
                x => x.Key,
                x => Math.Round(amount * x.Value, 4)
            );

            return new ConversionModel
            {
                SourceAmount = amount,
                SourceCurrency = sourceCurrency,
                Results = results
            };
        }
    }
}