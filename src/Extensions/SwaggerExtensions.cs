using Microsoft.OpenApi.Models;

namespace Playtesters.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithApiKey(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key needed to access the endpoints. X-Api-Key: Your_API_Key",
                In = ParameterLocation.Header,
                Name = "X-Api-Key",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        },
                        Scheme = "ApiKeyScheme",
                        Name = "X-Api-Key",
                        In = ParameterLocation.Header
                    },
                    []
                }
            });
        });
        return services;
    }
}
