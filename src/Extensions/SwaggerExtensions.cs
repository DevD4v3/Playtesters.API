using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Playtesters.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithApiKey(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Name = "X-Api-Key",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "API key required to access protected endpoints."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
            });

            options.OperationFilter<ApiKeySecurityOperationFilter>();
        });
        return services;
    }

    public class ApiKeySecurityOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            bool isAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

            if (isAnonymous)
            {
                operation.Security = [];
            }
        }
    }
}
