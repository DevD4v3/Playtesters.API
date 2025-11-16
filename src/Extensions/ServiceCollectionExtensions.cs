using Playtesters.API.UseCases.Testers;

namespace Playtesters.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateTesterUseCase>();
        services.AddSingleton<CreateTesterValidator>();
        return services;
    }
}
