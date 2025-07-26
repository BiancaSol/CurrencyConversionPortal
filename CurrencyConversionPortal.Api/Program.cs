using CurrencyConversionPortal.Api.DependencyInjection;
using CurrencyConversionPortal.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddCookieAuthentication();

builder.Services.AddCoreServices(); 

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
