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
            .Property(t => t.Name)
            .IsRequired()
            .HasColumnType("TEXT COLLATE NOCASE");

        modelBuilder
            .Entity<Tester>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder
            .Entity<Tester>()
            .HasIndex(t => t.AccessKey)
            .IsUnique();

        modelBuilder
            .Entity<AccessValidationHistory>()
            .HasIndex(h => h.CheckedAt);

        modelBuilder
            .Entity<AccessValidationHistory>()
            .Property(h => h.IpAddress)
            .IsRequired();

        modelBuilder
            .Entity<AccessValidationHistory>()
            .HasIndex(h => h.IpAddress);
    }
}
