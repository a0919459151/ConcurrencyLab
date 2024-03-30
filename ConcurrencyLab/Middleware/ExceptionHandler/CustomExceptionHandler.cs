using ConcurrencyLab.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace ConcurrencyLab.Middleware.ExceptionHandler;

public class CustomExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Handle AppException
        if (exception is AppException)
        {
            Console.WriteLine();
            Console.WriteLine(exception.Message);
            Console.WriteLine();

            return new ValueTask<bool>(true);
        }
        // Handle DbUpdateConcurrencyException
        else if (exception is DbUpdateConcurrencyException)
        {
            Console.WriteLine();
            Console.WriteLine("EF Core 發生 ConcurrencyException");
            Console.WriteLine();

            return new ValueTask<bool>(true);
        }
        // other exceptions
        else
        {
            Console.WriteLine();
            Console.WriteLine("Server error");
            Console.WriteLine();

            return new ValueTask<bool>(true);
        }
    }   
}
