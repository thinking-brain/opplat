using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Administration;

public sealed class SubscriptionPlan : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? Description { get; set; }

    [Required]
    public int MaxActiveUsers { get; set; }

    [Required]
    public decimal MaxApiCallsPerMonth { get; set; }

    [Required]
    public decimal MaxStorageGb { get; set; }

    [Required]
    public decimal PricingMonthly { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ResourceLimits { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Tenant> Tenants { get; } = [];
}
