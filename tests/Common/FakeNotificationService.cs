using Playtesters.API.Services;

namespace Playtesters.API.Tests.Common;

public class FakeNotificationService : INotificationService
{
    public Task SendAsync(NotificationMessage message) => Task.CompletedTask;
}
