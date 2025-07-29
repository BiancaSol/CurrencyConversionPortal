namespace CurrencyConversionPortal.Core.Services
{
    using CurrencyConversionPortal.Core.DataAccess;
    using CurrencyConversionPortal.Core.Entities;
    using CurrencyConversionPortal.Core.Exceptions;
    using CurrencyConversionPortal.Core.ExternalServices;
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
            try
            {
                return await _currencyData.GetCurrenciesAsync();
            }
            catch (Exception ex)
            {
                throw new ExternalServiceException("Failed to retrieve available currencies", ex);
            }
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
                throw new ExternalServiceException($"Currency conversion service is temporarily unavailable", ex);
            }
            catch (JsonException ex)
            {
                throw new ExternalServiceException($"Failed to process currency conversion response", ex);
            }
            catch (ValidationException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                throw new ExternalServiceException("An unexpected error occurred during currency conversion", ex);
            }
        }

        private void ValidateConversionRequest(decimal amount, string sourceCurrency)
        {
            if (amount <= 0)
            {
                throw new ValidationException("Amount must be greater than zero");
            }

            if (string.IsNullOrWhiteSpace(sourceCurrency))
            {
                throw new ValidationException("Source currency cannot be empty");
            }
        }

        private async Task<List<string>> GetCurrencyCodesAsync()
        {
            try
            {
                return (await _currencyData.GetCurrenciesAsync()).Select(c => c.Code).ToList();
            }
            catch (Exception ex)
            {
                throw new ExternalServiceException("Failed to retrieve available currency codes", ex);
            }
        }

        private void ValidateCurrencyCode(string currencyCode, List<string> availableCurrencies)
        {
            if (!availableCurrencies.Contains(currencyCode, StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException($"Invalid source currency: {currencyCode}. Please use a supported currency code.");
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