namespace CurrencyConversionPortal.Api.DTOs
{
    using System.Collections.Generic;

    public class ConversionResponseDto
    {
        public decimal SourceAmount { get; set; }
        public string SourceCurrency { get; set; } = string.Empty;
        public Dictionary<string, decimal> ConvertedAmounts { get; set; } = new();
    }
}