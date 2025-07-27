namespace CurrencyConversionPortal.Core.DataAccess
{
    using Entities;
    using Microsoft.Extensions.Caching.Memory;

    public class UserData : IUserData
    {
        private readonly IMemoryCache _memoryCache;
        private const string UsersCacheKey = "users";

        public UserData(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public User? GetById(Guid id)
        {
            var users = _memoryCache.Get<List<User>>(UsersCacheKey) ?? new List<User>();
            return users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            var users = _memoryCache.Get<List<User>>(UsersCacheKey) ?? new List<User>();
            users.Add(user);
            _memoryCache.Set(UsersCacheKey, users);
        }

        public IEnumerable<User> GetAll()
        {
            return _memoryCache.Get<List<User>>(UsersCacheKey) ?? new List<User>();
        }
    }
}
