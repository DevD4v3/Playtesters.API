using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Playtesters.API.Entities;

namespace Playtesters.API.Data;

public class IpGeoCacheConfiguration : IEntityTypeConfiguration<IpGeoCache>
{
    public void Configure(EntityTypeBuilder<IpGeoCache> builder)
    {
        builder
            .Property(i => i.IpAddress)
            .IsRequired();

        builder
            .Property(i => i.Country)
            .IsRequired();

        builder
            .Property(i => i.City)
            .IsRequired();

        builder
            .HasIndex(t => t.IpAddress)
            .IsUnique();
    }
}
