using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Core.Exceptions;

namespace UnitConversion.Api.Middleware;

//Maps known domain exceptions to RFC 7807 problem responses; everything else becomes a 500.
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            UnknownUnitException ex => (StatusCodes.Status400BadRequest, "Unknown unit", ex.Message),
            OverflowException => (StatusCodes.Status400BadRequest, "Value out of range",
                "The value is too large to convert."),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error",
                "An unexpected error occurred.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = title, Detail = detail },
            cancellationToken);
        return true;
    }
}
