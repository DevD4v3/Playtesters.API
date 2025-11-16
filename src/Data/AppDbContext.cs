using Microsoft.EntityFrameworkCore;
using Playtesters.API.Entities;

namespace Playtesters.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tester>()
            .HasIndex(t => t.UserName)
            .IsUnique();
    }
}
