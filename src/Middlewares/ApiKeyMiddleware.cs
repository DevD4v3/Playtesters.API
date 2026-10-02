using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using DotEnv.Core;
using SimpleResults;

namespace Playtesters.API.Middlewares;

public class ApiKeyMiddleware(
    IEnvReader envReader,
    RequestDelegate next)
{
    private readonly byte[] _apiKeyBytes = Encoding.UTF8.GetBytes(envReader["API_KEY"]);

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var providedKey))
        {
            await Unauthorized(context, "Missing API Key.");
            return;
        }

        var providedKeyBytes = Encoding.UTF8.GetBytes(providedKey.ToString());

        if (!CryptographicOperations.FixedTimeEquals(_apiKeyBytes, providedKeyBytes))
        {
            await Unauthorized(context, "Invalid API Key.");
            return;
        }

        await next(context);
    }

    private static async Task Unauthorized(HttpContext context, string message)
    {
        Result result = Result.Unauthorized(message);
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(result);
    }
}
