using CurrencyConversionPortal.Api.DependencyInjection;
using CurrencyConversionPortal.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddCookieAuthentication();

builder.Services.AddCoreServices(builder.Configuration);

builder.Services.AddHttpClient();

var app = builder.Build();

app.AddExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
