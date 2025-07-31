using CurrencyConversionPortal.Api.DependencyInjection;
using CurrencyConversionPortal.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsPolicies(builder.Environment);

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddCookieAuthentication(builder.Environment);

builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.AddExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
