namespace CurrencyConversionPortal.Core.Services
{
    using BCrypt.Net;
    using CurrencyConversionPortal.Core.DataAccess;
    using CurrencyConversionPortal.Core.Entities;

    public class UserService : IUserService
    {
        private readonly IUserData _userData;

        public UserService(IUserData userData)
        {
            _userData = userData;
        }

        public bool Register(string userName, string password)
        {
            if (_userData.GetAll().Any(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = userName,
                Password = BCrypt.HashPassword(password)
            };

            _userData.AddUser(newUser);

            return true;
        }

        public bool ValidateCredentials(string userName, string password)
        {
            var user = _userData.GetAll()
                .FirstOrDefault(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (user == null || !BCrypt.Verify(password, user.Password))
            {
                return false;
            }

            return true;
        }
    }
}