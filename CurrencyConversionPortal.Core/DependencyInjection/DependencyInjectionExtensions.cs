namespace CurrencyConversionPortal.Core.DependencyInjection
{
    using CurrencyConversionPortal.Core.Configuration;
    using CurrencyConversionPortal.Core.DataAccess;
    using CurrencyConversionPortal.Core.ExternalServices;
    using CurrencyConversionPortal.Core.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Data Access Layer
            services.AddScoped<IUserData, UserData>();
            services.AddScoped<ICurrencyData, CurrencyData>();

            // Business Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrencyService, CurrencyService>();

            // External API Services
            services.AddScoped<ICurrencyConversionApiClient, CurrencyConversionApiClient>();
            services.AddCurrencyApiHttpClient(configuration);

            return services;
        }

        private static IServiceCollection AddCurrencyApiHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(nameof(ICurrencyConversionApiClient), client =>
            {
                var baseUrl = configuration.GetSection(CurrencyApiConfiguration.SectionName)
                    .GetValue<string>(CurrencyApiConfiguration.BaseUrlProperty);
                client.BaseAddress = new Uri(baseUrl!);
            });

            return services;
        }
    }
}
