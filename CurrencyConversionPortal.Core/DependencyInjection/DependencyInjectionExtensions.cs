namespace CurrencyConversionPortal.Core.DependencyInjection
{
    using CurrencyConversionPortal.Core.DataAccess;
    using CurrencyConversionPortal.Core.Services;
    using CurrencyConversionPortal.Core.Services.Api;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IUserData, UserData>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrencyConversionApiClient, CurrencyConversionApiClient>();
            services.AddScoped<ICurrencyData, CurrencyData>();
            services.AddScoped<ICurrencyService, CurrencyService>();

            return services;
        }
    }
}
