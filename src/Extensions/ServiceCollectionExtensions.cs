using Playtesters.API.Services;
using Playtesters.API.UseCases.TesterAccessHistory;
using Playtesters.API.UseCases.Testers;

namespace Playtesters.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<CreateTesterUseCase>()
            .AddScoped<UpdateTesterUseCase>()
            .AddScoped<UpdatePlaytimeUseCase>()
            .AddScoped<GetTestersUseCase>()
            .AddScoped<ValidateTesterAccessUseCase>()
            .AddScoped<GetAllTestersAccessHistoryUseCase>()
            .AddScoped<RevokeAllKeysUseCase>();

        services.AddHttpContextAccessor();
        services.AddScoped<IClientIpProvider, ClientIpProvider>();
        services
            .AddHttpClient<IIpGeoLocationService, IpGeoLocationService>(httpClient =>
            {
                httpClient.Timeout = TimeSpan.FromSeconds(5);
            });

        services
            .AddSingleton<CreateTesterValidator>()
            .AddSingleton<UpdateTesterValidator>()
            .AddSingleton<UpdatePlaytimeValidator>()
            .AddSingleton<ValidateTesterAccessValidator>()
            .AddSingleton<GetTestersValidator>()
            .AddSingleton<GetAllTestersAccessHistoryValidator>();

        return services;
    }
}
