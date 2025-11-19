using Microsoft.AspNetCore.Diagnostics;
using SimpleResults;
using System.Net;

namespace Playtesters.API.ExceptionHandlers;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        var response = Result.CriticalError();
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
