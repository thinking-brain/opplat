namespace Opplat.Domain.Entities.Administration;

public sealed class AuditLog : BaseEntity
{
    public string ActorOid { get; set; } = string.Empty;

    public string? TargetTenantId { get; set; }

    public string? TargetTenantIdFk { get; set; }

    public string? TargetUserId { get; set; }

    public TenantUser? TargetUser { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string? BeforeState { get; set; }

    public string? AfterState { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public TenantUser? Actor { get; set; }

    public Tenant? TargetTenant { get; set; }
}
