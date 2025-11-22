using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Playtesters.API.Entities;

namespace Playtesters.API.Data;

public class AccessHistoryConfiguration 
    : IEntityTypeConfiguration<AccessValidationHistory>
{
    public void Configure(EntityTypeBuilder<AccessValidationHistory> builder)
    {
        builder
            .Property(h => h.CheckedAt)
            .IsRequired();

        builder
            .Property(h => h.IpAddress)
            .IsRequired();

        builder
            .Property(h => h.Country)
            .IsRequired()
            .HasColumnType("TEXT COLLATE NOCASE");

        builder
            .Property(h => h.City)
            .IsRequired()
            .HasColumnType("TEXT COLLATE NOCASE");

        builder.HasIndex(h => h.CheckedAt);
        builder.HasIndex(h => h.IpAddress);
        builder.HasIndex(h => h.Country);
        builder.HasIndex(h => h.City);
    }
}
