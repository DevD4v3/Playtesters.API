using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;

namespace Playtesters.API.Extensions;

public static class MigrationExtensions
{
    public static async Task MigrateDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
}
