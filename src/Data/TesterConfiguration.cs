using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Playtesters.API.Entities;

namespace Playtesters.API.Data;

public class TesterConfiguration : IEntityTypeConfiguration<Tester>
{
    public void Configure(EntityTypeBuilder<Tester> builder)
    {
        builder
            .Property(t => t.Name)
            .IsRequired()
            .HasColumnType("TEXT COLLATE NOCASE");

        builder
            .Property(t => t.TotalHoursPlayed)
            .IsRequired();

        builder
            .HasIndex(t => t.Name)
            .IsUnique();

        builder
            .HasIndex(t => t.AccessKey)
            .IsUnique();
    }
}
