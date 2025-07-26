namespace CurrencyConversionPortal.Core.DependencyInjection
{
    using CurrencyConversionPortal.Core.DataAccess;
    using CurrencyConversionPortal.Core.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IUserData, UserData>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
