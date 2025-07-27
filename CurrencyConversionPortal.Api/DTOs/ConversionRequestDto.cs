namespace CurrencyConversionPortal.Api.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class ConversionRequestDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        public string SourceCurrency { get; set; } = string.Empty;
    }
}