namespace CurrencyConversionPortal.Api.DependencyInjection
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    if (environment.IsDevelopment())
                    {
                        policy.WithOrigins("http://localhost:4200");
                    }
                    else
                    {
                        policy.WithOrigins("https://myportaldomain.com");
                    }
                    
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}