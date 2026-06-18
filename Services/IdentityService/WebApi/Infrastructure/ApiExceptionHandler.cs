using Application.Shared.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Infrastructure;

public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ValidationException or ArgumentException => (StatusCodes.Status400BadRequest, "Validation failed"),
            UnauthorizedException or UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            EntityNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        if (status >= 500)
        {
            logger.LogError(exception, "Unhandled IdentityService exception");
        }
        else
        {
            logger.LogWarning(exception, "IdentityService request failed with status {StatusCode}", status);
        }

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status >= 500 ? null : exception.Message,
                Instance = httpContext.Request.Path
            }
        });
    }
}
