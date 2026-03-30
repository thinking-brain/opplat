using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Administration;

public enum TenantStatus
{
    Active,
    Inactive
}

public sealed class Tenant : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TenantStatus Status { get; set; } = TenantStatus.Active;

    [Required]
    [ForeignKey(nameof(SubscriptionPlan))]
    public Guid SubscriptionPlanId { get; set; }

    public DateTime? InactivatedAt { get; set; }

    [Required]
    [ForeignKey(nameof(DatabaseInstance))]
    public Guid DatabaseInstanceId { get; set; }

    [MaxLength(128)]
    public required string DatabaseName { get; set; }
    public required string DatabaseSchema { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public DatabaseInstance? DatabaseInstance { get; set; }

    public ICollection<TenantUser> TenantUsers { get; } = [];

    public ICollection<AuditLog> AuditLogs { get; } = [];

    public int UserCount => TenantUsers.Count;

    public bool IsActive => Status == TenantStatus.Active;
}
