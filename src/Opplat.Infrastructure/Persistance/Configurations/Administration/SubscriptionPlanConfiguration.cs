using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("subscription_plans");

        builder.HasKey(t => t.Id);

        builder.Property(p => p.Id).UseIdentityByDefaultColumn();
        builder.Property(p => p.Name).HasMaxLength(128).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(512);
        builder.Property(p => p.MaxActiveUsers).IsRequired();
        builder.Property(p => p.MaxApiCallsPerMonth).IsRequired();
        builder.Property(p => p.MaxStorageGb).IsRequired();
        builder.Property(p => p.PricingMonthly).IsRequired();
        builder.Property(p => p.ResourceLimits).HasColumnType("jsonb");
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.UtcNow);
        builder.HasMany(p => p.Tenants)
            .WithOne(t => t.SubscriptionPlan)
            .HasForeignKey(t => t.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}