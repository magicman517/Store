using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Common.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, errorCode) = MapToStatusAndTitle(exception);

        httpContext.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Type = errorCode ?? exception.GetType().Name,
                Title = "An error occurred while processing your request",
                Status = status,
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            }
        });
    }

    private static (int Status, string? ErrorCode) MapToStatusAndTitle(Exception ex)
    {
        if (ex is ApiException apiException)
        {
            return (apiException.StatusCode, apiException.ErrorCode);
        }
        return (500, null);
    }
}