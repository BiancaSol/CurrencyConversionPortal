namespace CurrencyConversionPortal.Core.Models.Api
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class CurrencyRatesResponse
    {
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}