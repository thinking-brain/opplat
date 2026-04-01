namespace Opplat.Domain.Entities.Administration;

public enum TenantStatus
{
    Active,
    Inactive
}

public sealed class Tenant : BaseEntity
{
    public string Identifier { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TenantStatus Status { get; set; } = TenantStatus.Active;

    public Guid SubscriptionPlanId { get; set; }

    public DateTime? InactivatedAt { get; set; }

    public Guid DatabaseInstanceId { get; set; }

    public required string DatabaseName { get; set; }
    public required string DatabaseSchema { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public DatabaseInstance? DatabaseInstance { get; set; }

    public ICollection<TenantUser> TenantUsers { get; } = [];

    public ICollection<AuditLog> AuditLogs { get; } = [];

    public int UserCount => TenantUsers.Count;

    public bool IsActive => Status == TenantStatus.Active;
}
