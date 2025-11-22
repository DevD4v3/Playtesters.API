using Microsoft.EntityFrameworkCore;

namespace Playtesters.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TesterConfiguration());
        modelBuilder.ApplyConfiguration(new AccessHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new IpGeoCacheConfiguration());
    }
}
