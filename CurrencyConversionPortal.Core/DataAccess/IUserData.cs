namespace CurrencyConversionPortal.Core.DataAccess
{
    using Entities;

    public interface IUserData
    {
        User? GetById(Guid id);
        void AddUser(User user);
        IEnumerable<User> GetAll();
    }
}
