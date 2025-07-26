namespace CurrencyConversionPortal.Core.Services
{
    public interface IUserService
    {
        bool Register(string userName, string password);

        bool ValidateCredentials(string userName, string password);
    }
}