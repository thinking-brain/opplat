using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(t => t.Id);

        builder.Property(l => l.Id).UseIdentityByDefaultColumn();
        builder.Property(l => l.ActorOid).HasMaxLength(128).IsRequired();
        builder.Property(l => l.TargetTenantId).HasMaxLength(128);
        builder.Property(l => l.ActionType).HasMaxLength(128).IsRequired();
        builder.Property(l => l.BeforeState).HasColumnType("jsonb");
        builder.Property(l => l.AfterState).HasColumnType("jsonb");
        builder.Property(l => l.Timestamp).HasDefaultValue(DateTime.UtcNow);
        builder.HasOne(l => l.TargetUser)
            .WithMany(u => u.TargetUserAuditLogs)
            .HasForeignKey(l => l.TargetUserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(l => l.TargetTenant)
            .WithMany(t => t.AuditLogs)
            .HasForeignKey(l => l.TargetTenantIdFk)
            .OnDelete(DeleteBehavior.Restrict);
    }
}