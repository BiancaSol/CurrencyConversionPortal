namespace CurrencyConversionPortal.Api.DependencyInjection
{
    using Microsoft.AspNetCore.Authentication.Cookies;

    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
                options.LoginPath = "/api/auth/login";
                options.LogoutPath = "/api/auth/logout";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("StandardUser", policy =>
                    policy.RequireAuthenticatedUser());
            });

            return services;
        }
    }
}
