using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class DatabaseInstanceConfiguration : IEntityTypeConfiguration<DatabaseInstance>
{
    public void Configure(EntityTypeBuilder<DatabaseInstance> builder)
    {
        builder.ToTable("database_instances");

        builder.Property(d => d.Identifier).HasMaxLength(128).IsRequired();
        builder.Property(d => d.ConnectionStringReference).HasMaxLength(512).IsRequired();
        builder.Property(d => d.CurrentTenantSchemaCount).IsRequired();
        builder.Property(d => d.Status).HasConversion<string>();
        builder.Property(d => d.CreatedAt).HasDefaultValue(DateTime.UtcNow);
        builder.HasMany(d => d.Tenants)
            .WithOne(t => t.DatabaseInstance)
            .HasForeignKey(t => t.DatabaseInstanceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}