using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConcurrencyLab.Middleware.ExceptionHandler;

public class CustomExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Handle AppException
        if (exception is AppException)
        {
            // Response 400
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            };

            var json = JsonSerializer.Serialize(problemDetails);

            httpContext.Response.WriteAsync(json, cancellationToken);

            Console.WriteLine(exception.Message);
        }
        // Other exceptions
        else
        {
            // Response 500
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Server error",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            };

            var json = JsonSerializer.Serialize(problemDetails);

            httpContext.Response.WriteAsync(json, cancellationToken);

            Console.WriteLine("Server error");
        }

        return new ValueTask<bool>(true);
    }
}
