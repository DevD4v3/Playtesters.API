using DotEnv.Core;
using SimpleResults;
using System.Net;

namespace Playtesters.API.Middlewares;

public class ApiKeyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && path.StartsWith("/api/tester/validate-access"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var providedKey))
        {
            var response = Result.Unauthorized("Missing API Key.");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        var envReader = new EnvReader();
        var apiKey = envReader["API_KEY"];
        if (!apiKey.Equals(providedKey))
        {
            var response = Result.Unauthorized("Invalid API Key.");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await next(context);
    }
}
