using CurrencyConversionPortal.Api.DependencyInjection;
using CurrencyConversionPortal.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddCookieAuthentication();

builder.Services.AddCoreServices();

builder.Services.AddHttpClient();

var app = builder.Build();

app.AddExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
