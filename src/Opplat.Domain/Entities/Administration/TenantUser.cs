namespace Opplat.Domain.Entities.Administration;

public enum TenantUserRole
{
    PrimaryAdmin,
    Admin,
    User
}

public sealed class TenantUser : BaseEntity
{
    public string EntraOid { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public string Email { get; set; } = string.Empty;

    public TenantUserRole Role { get; set; } = TenantUserRole.User;

    public bool IsPrimaryAdmin { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime? DeactivatedAt { get; set; }

    public Tenant? Tenant { get; set; }

    public ICollection<AuditLog> ActorAuditLogs { get; } = [];

    public ICollection<AuditLog> TargetUserAuditLogs { get; } = [];
}
