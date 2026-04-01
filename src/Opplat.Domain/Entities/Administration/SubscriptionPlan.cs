namespace Opplat.Domain.Entities.Administration;

public sealed class SubscriptionPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int MaxActiveUsers { get; set; }

    public decimal MaxApiCallsPerMonth { get; set; }

    public decimal MaxStorageGb { get; set; }

    public decimal PricingMonthly { get; set; }

    public string? ResourceLimits { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Tenant> Tenants { get; } = [];
}
