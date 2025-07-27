namespace CurrencyConversionPortal.Core.DataAccess
{
    using CurrencyConversionPortal.Core.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICurrencyData
    {
        Task<IEnumerable<Currency>> GetCurrenciesAsync();
    }
}
