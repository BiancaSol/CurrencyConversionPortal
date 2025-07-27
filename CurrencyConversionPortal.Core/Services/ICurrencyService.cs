namespace CurrencyConversionPortal.Core.Services
{
    using CurrencyConversionPortal.Core.Entities;
    using CurrencyConversionPortal.Core.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetCurrenciesAsync();
        Task<ConversionModel> ConvertAsync(decimal amount, string sourceCurrency);
    }
}