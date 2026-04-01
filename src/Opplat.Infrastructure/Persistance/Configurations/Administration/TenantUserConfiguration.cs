using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.ToTable("tenant_users");

        builder.HasKey(t => t.Id);

        builder.Property(u => u.EntraOid).HasMaxLength(128).IsRequired();
        builder.Property(u => u.TenantId).HasMaxLength(128).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().IsRequired();
        builder.Property(u => u.IsPrimaryAdmin).IsRequired();
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.CreatedAt).HasDefaultValue(DateTime.UtcNow);
        builder.HasIndex(u => new { u.TenantId, u.EntraOid }).IsUnique();
        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.TenantUsers)
            .HasForeignKey(u => u.TenantId)
            .HasPrincipalKey(t => t.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}