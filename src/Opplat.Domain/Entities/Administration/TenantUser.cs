using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Administration;

public enum TenantUserRole
{
    PrimaryAdmin,
    Admin,
    User
}

public sealed class TenantUser : BaseEntity
{
    [Required]
    [MaxLength(128)]
    public string EntraOid { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(Tenant))]
    [MaxLength(128)]
    public Guid TenantId { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public TenantUserRole Role { get; set; } = TenantUserRole.User;

    public bool IsPrimaryAdmin { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime? DeactivatedAt { get; set; }

    public Tenant? Tenant { get; set; }

    public ICollection<AuditLog> ActorAuditLogs { get; } = [];

    public ICollection<AuditLog> TargetUserAuditLogs { get; } = [];
}
