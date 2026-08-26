namespace Opplat.Domain.Entities.Administration;

public enum TenantStatus
{
    Active,
    Inactive
}

public enum TenantBillingStatus
{
    Trialing,
    Active,
    PastDue,
    Cancelled
}

public enum BillingInterval
{
    Monthly,
    Annual
}

public sealed class Tenant : BaseEntity
{
    public string Identifier { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TenantStatus Status { get; set; } = TenantStatus.Active;

    public TenantBillingStatus BillingStatus { get; set; } = TenantBillingStatus.Active;

    public BillingInterval BillingInterval { get; set; } = BillingInterval.Monthly;

    public Guid SubscriptionPlanId { get; set; }

    public DateTime? InactivatedAt { get; set; }

    public DateTime? NextBillingDate { get; set; }

    public string? StripeCustomerId { get; set; }

    public string? StripeSubscriptionId { get; set; }

    public bool CancelAtPeriodEnd { get; set; }

    public Guid DatabaseInstanceId { get; set; }

    public required string DatabaseSchema { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public DatabaseInstance? DatabaseInstance { get; set; }

    public ICollection<TenantUser> TenantUsers { get; set; } = [];

    public ICollection<AuditLog> AuditLogs { get; } = [];

    public int UserCount => TenantUsers.Count;

    public bool IsActive => Status == TenantStatus.Active;
}
