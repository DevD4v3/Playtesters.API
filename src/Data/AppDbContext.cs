using Microsoft.EntityFrameworkCore;
using Playtesters.API.Entities;

namespace Playtesters.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Tester>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder
            .Entity<AccessValidationHistory>()
            .HasIndex(h => h.CheckedAt);

        modelBuilder
            .Entity<AccessValidationHistory>()
            .HasIndex(h => h.IpAddress);
    }
}
