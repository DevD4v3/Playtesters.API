using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Playtesters.API.Services;

public record GeoLocationResponse(
    string Country = "Unknown",
    string City = "Unknown"
);

public interface IIpGeoLocationService
{
    Task<GeoLocationResponse> GetLocationAsync(string ipAddress);
}

public class IpGeoLocationService(
    AppDbContext dbContext,
    HttpClient httpClient,
    ILogger<IpGeoLocationService> logger) : IIpGeoLocationService
{
    private record IpApiResponse(
        [property: JsonPropertyName("status")]string Status,
        [property: JsonPropertyName("country")]string Country,
        [property: JsonPropertyName("city")]string City
    );

    public async Task<GeoLocationResponse> GetLocationAsync(string ipAddress)
    {
        if (IsLocalIp(ipAddress))
            return new GeoLocationResponse();

        var cached = await dbContext
            .Set<IpGeoCache>()
            .FirstOrDefaultAsync(i => i.IpAddress == ipAddress);

        if (cached is not null)
            return new GeoLocationResponse(cached.Country, cached.City);

        HttpResponseMessage response;
        try
        {
            var url = $"http://ip-api.com/json/{ipAddress}";
            response = await httpClient.GetAsync(url);
        }
        catch (Exception ex) when 
            (ex is TaskCanceledException or HttpRequestException)
        {
            logger.LogError(ex, "IP-API request failed for IP {IpAddress}", ipAddress);
            return new GeoLocationResponse();
        }

        if (!response.IsSuccessStatusCode)
            return new GeoLocationResponse();

        var json = await response.Content.ReadAsStringAsync();
        var ipApiResponse = JsonSerializer.Deserialize<IpApiResponse>(json);
        if (ipApiResponse is null || ipApiResponse.Status != "success")
            return new GeoLocationResponse();

        var ipGeoCache = new IpGeoCache
        {
            IpAddress = ipAddress,
            Country = ipApiResponse.Country,
            City = ipApiResponse.City
        };

        dbContext.Add(ipGeoCache);
        await dbContext.SaveChangesAsync();
        return new GeoLocationResponse(ipApiResponse.Country, ipApiResponse.City);
    }

    private static bool IsLocalIp(string ipAddress)
    {
        return ipAddress == "127.0.0.1"
            || ipAddress == "::1"
            || ipAddress.StartsWith("::ffff:127.");
    }
}
