using DotEnv.Core;
using Playtesters.API.Extensions;

namespace Playtesters.API.Services;

public interface INotificationService
{
    Task SendAsync(NotificationMessage message);
}

public record NotificationMessage(
    string TesterName, 
    string IpAddress, 
    string Country, 
    string City,
    double HoursPlayed,
    DateTime Timestamp
);

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _discordWebhookUrl;
    private record DiscordWebhookPayload(string Content);

    public NotificationService(
        HttpClient httpClient,
        ILogger<NotificationService> logger)
    {
        var envReader = new EnvReader();
        if (!envReader.TryGetStringValue("DISCORD_WEBHOOK_URL", out var webhookUrl))
        {
            logger.LogError("'DISCORD_WEBHOOK_URL' has not been set as an environment variable");
        }
        _discordWebhookUrl = webhookUrl ?? string.Empty;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendAsync(NotificationMessage message)
    {
        if (string.IsNullOrWhiteSpace(_discordWebhookUrl))
        {
            _logger.LogError("Discord webhook URL is not configured. Skipping notification for Tester {TesterName}", message.TesterName);
            return;
        }

        var content = 
        $"""
        🔔 *New validated access:*
        👤 Tester: **{message.TesterName}**
        🌐 IP: `{message.IpAddress}`
        🌍 Country: {message.Country}
        🏙️ City: {message.City}
        ⏳ Hours played: {message.HoursPlayed.ToHhMmSs()}
        ⏰ {message.Timestamp:yyyy-MM-dd HH:mm:ss}

        """;

        try
        {
            var payload = new DiscordWebhookPayload(content);
            var response = await _httpClient.PostAsJsonAsync(_discordWebhookUrl, payload);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
        {
            _logger.LogError(ex, "DiscordWebhook timed out for Tester {TesterName}", message.TesterName);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "DiscordWebhook HTTP error for Tester {TesterName}", message.TesterName);
        }
    }
}
