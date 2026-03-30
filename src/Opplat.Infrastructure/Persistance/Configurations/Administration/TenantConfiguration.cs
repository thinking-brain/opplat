using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasMaxLength(128);
        builder.Property(t => t.Identifier).HasMaxLength(128).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Status).HasConversion<string>().IsRequired();
        builder.Property(t => t.SubscriptionPlanId).IsRequired();
        builder.Property(t => t.CreatedAt).HasDefaultValue(DateTime.UtcNow);
        builder.Property(t => t.DatabaseInstanceId).IsRequired();
        builder.Property(t => t.DatabaseSchema).HasMaxLength(128);
        builder.HasIndex(t => t.Identifier).IsUnique();
        builder.HasOne(t => t.SubscriptionPlan)
            .WithMany(p => p.Tenants)
            .HasForeignKey(t => t.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.DatabaseInstance)
            .WithMany(d => d.Tenants)
            .HasForeignKey(t => t.DatabaseInstanceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}