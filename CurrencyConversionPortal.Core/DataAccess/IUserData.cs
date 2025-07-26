namespace CurrencyConversionPortal.Core.DataAccess
{
    using CurrencyConversionPortal.Core.Models;

    public interface IUserData
    {
        User? GetById(Guid id);
        void AddUser(User user);
        IEnumerable<User> GetAll();
    }
}
