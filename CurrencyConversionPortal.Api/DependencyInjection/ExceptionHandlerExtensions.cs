namespace CurrencyConversionPortal.Api.DependencyInjection
{
    using System.Text.Json;

    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new { error = "An internal server error occurred." });
                    await context.Response.WriteAsync(result);
                });
            });

            return app;
        }
    }
}
