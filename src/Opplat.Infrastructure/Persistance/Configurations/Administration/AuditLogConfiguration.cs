using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.Property(l => l.ActorOid).HasMaxLength(128).IsRequired();
        builder.Property(l => l.TargetTenantId).HasMaxLength(128);
        builder.Property(l => l.TargetTenantIdFk).HasMaxLength(128);
        builder.Property(l => l.ActionType).HasMaxLength(128).IsRequired();
        builder.Property(l => l.BeforeState).HasColumnType("jsonb");
        builder.Property(l => l.AfterState).HasColumnType("jsonb");
        builder.Property(l => l.Timestamp).HasDefaultValue(DateTime.UtcNow);
        builder.HasOne(l => l.Actor)
            .WithMany(u => u.ActorAuditLogs)
            .HasForeignKey(l => l.ActorOid)
            .HasPrincipalKey(u => u.EntraOid)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.TargetUser)
            .WithMany(u => u.TargetUserAuditLogs)
            .HasForeignKey(l => l.TargetUserId)
            .HasPrincipalKey(u => u.EntraOid)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(l => l.TargetTenant)
            .WithMany(t => t.AuditLogs)
            .HasForeignKey(l => l.TargetTenantIdFk)
            .HasPrincipalKey(t => t.Identifier)
            .OnDelete(DeleteBehavior.Restrict);
    }
}