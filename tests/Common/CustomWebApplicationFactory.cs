using Microsoft.AspNetCore.Mvc.Testing;
using Playtesters.API.Services;

namespace Playtesters.API.Tests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        Environment.SetEnvironmentVariable("API_KEY", "Test123");
        Environment.SetEnvironmentVariable("SQLITE_DATA_SOURCE", "playtesters.db");

        builder.ConfigureServices(services =>
        {
            var clientIpProviderDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IClientIpProvider));

            var ipGeoLocationServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IIpGeoLocationService));

            var notificationServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(INotificationService));

            services.Remove(clientIpProviderDescriptor);
            services.Remove(ipGeoLocationServiceDescriptor);
            services.Remove(notificationServiceDescriptor);
            services.AddSingleton<IClientIpProvider, FakeClientIpProvider>();
            services.AddSingleton<IIpGeoLocationService, FakeIpGeoLocationService>();
            services.AddSingleton<INotificationService, FakeNotificationService>();
        });
    }
}
