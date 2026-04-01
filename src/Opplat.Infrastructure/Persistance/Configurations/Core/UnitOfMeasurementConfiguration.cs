using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities;

namespace Opplat.Infrastructure.Persistance.Configurations.Core;

public sealed class UnitOfMeasurementConfiguration : IEntityTypeConfiguration<UnitOfMeasurement>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasurement> builder)
    {
        builder.HasKey(u => u.Abbreviation);
        builder.Property(u => u.Abbreviation).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Name).IsRequired();
        builder.Property(u => u.UnitType).HasConversion<string>();
        builder.Property(u => u.CovertionFactor).IsRequired();
    }
}
