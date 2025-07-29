namespace CurrencyConversionPortal.Api.DependencyInjection
{
    using CurrencyConversionPortal.Api.Models;
    using CurrencyConversionPortal.Core.Exceptions;
    using Microsoft.AspNetCore.Diagnostics;
    using System.Net;
    using System.Text.Json;

    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionHandlerFeature?.Error;

                    var response = CreateErrorResponse(exception);
                    
                    context.Response.StatusCode = response.StatusCode;
                    context.Response.ContentType = "application/json";

                    var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    await context.Response.WriteAsync(jsonResponse);
                });
            });

            return app;
        }

        private static ApiErrorResponse CreateErrorResponse(Exception? exception)
        {
            return exception switch
            {
                ValidationException validationEx => new ApiErrorResponse
                {
                    Error = "Validation failed",
                    Details = validationEx.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                },
                
                CurrencyConversionException businessEx => new ApiErrorResponse
                {
                    Error = "Business logic error",
                    Details = businessEx.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                },
                
                ExternalServiceException serviceEx => new ApiErrorResponse
                {
                    Error = "External service error",
                    Details = "Currency conversion service is temporarily unavailable",
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable
                },
                
                ArgumentException argEx => new ApiErrorResponse
                {
                    Error = "Invalid request",
                    Details = argEx.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                },
                
                UnauthorizedAccessException => new ApiErrorResponse
                {
                    Error = "Unauthorized",
                    Details = "Access denied",
                    StatusCode = (int)HttpStatusCode.Unauthorized
                },
                
                _ => new ApiErrorResponse
                {
                    Error = "An internal server error occurred",
                    Details = null, // Don't expose internal error details in production
                    StatusCode = (int)HttpStatusCode.InternalServerError
                }
            };
        }
    }
}
