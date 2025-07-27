namespace CurrencyConversionPortal.Core.Models
{ 
    using System.Collections.Generic;
    
    public class ConversionModel
    {
        public decimal SourceAmount { get; set; }
        public string SourceCurrency { get; set; }
        public Dictionary<string, decimal> Results { get; set; }
    }
}
