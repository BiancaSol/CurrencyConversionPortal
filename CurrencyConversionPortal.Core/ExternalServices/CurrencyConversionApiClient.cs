namespace CurrencyConversionPortal.Core.ExternalServices
{
    using CurrencyConversionPortal.Core.Models.Api;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CurrencyConversionApiClient : ICurrencyConversionApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CurrencyConversionApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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

            using var httpClient = _httpClientFactory.CreateClient(nameof(ICurrencyConversionApiClient));

            var symbols = string.Join(",", targetCurrencies);
            var requestUrl = $"latest?base={sourceCurrency}&symbols={symbols}";

            var response = await httpClient.GetAsync(requestUrl);
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
            using var httpClient = _httpClientFactory.CreateClient(nameof(ICurrencyConversionApiClient));
            const string requestUrl = "currencies";

            var response = await httpClient.GetAsync(requestUrl);
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