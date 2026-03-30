using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Administration;

public sealed class AuditLog : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string ActorOid { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? TargetTenantId { get; set; }

    [MaxLength(128)]
    public string? TargetTenantIdFk { get; set; }

    public string? TargetUserId { get; set; }

    [ForeignKey(nameof(TargetUserId))]
    public TenantUser? TargetUser { get; set; }

    [Required]
    [MaxLength(128)]
    public string ActionType { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string? BeforeState { get; set; }

    [Column(TypeName = "jsonb")]
    public string? AfterState { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public TenantUser? Actor { get; set; }

    [ForeignKey(nameof(TargetTenantIdFk))]
    public Tenant? TargetTenant { get; set; }
}
