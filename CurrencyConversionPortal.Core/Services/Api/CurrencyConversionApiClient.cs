namespace CurrencyConversionPortal.Core.Services.Api
{
    using CurrencyConversionPortal.Core.Models.Api;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CurrencyConversionApiClient : ICurrencyConversionApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://api.frankfurter.dev/v1";

        public CurrencyConversionApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CurrencyRatesResponse> GetConversionRatesAsync(string sourceCurrency, List<string> targetCurrencies)
        {
            if (string.IsNullOrWhiteSpace(sourceCurrency))
            {
                throw new ArgumentException("Source currency cannot be empty", nameof(sourceCurrency));
            }

            if (targetCurrencies == null || targetCurrencies.Count == 0)
            {
                throw new ArgumentException("Target currencies list cannot be empty", nameof(targetCurrencies));
            }

            var symbols = string.Join(",", targetCurrencies);
            var requestUrl = $"{_apiBaseUrl}/latest?base={sourceCurrency}&symbols={symbols}";

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = await JsonSerializer.DeserializeAsync<CurrencyRatesResponse>(stream, jsonOptions);

            if (apiResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize the API response");
            }

            return apiResponse;
        }

        public async Task<Dictionary<string, string>> GetAvailableCurrenciesAsync()
        {
            var requestUrl = $"{_apiBaseUrl}/currencies";
            
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var currencies = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream, jsonOptions);

            return currencies ?? new Dictionary<string, string>();
        }
    }
}