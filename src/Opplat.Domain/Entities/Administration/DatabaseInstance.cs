using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Administration;

public enum DatabaseInstanceStatus
{
    Active,
    Archived
}

public sealed class DatabaseInstance : BaseEntity
{

    [Required]
    [MaxLength(128)]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    [MaxLength(512)]
    public string ConnectionStringReference { get; set; } = string.Empty;

    [Required]
    public int CurrentTenantSchemaCount { get; set; } = 0;

    [Required]
    public DatabaseInstanceStatus Status { get; set; } = DatabaseInstanceStatus.Active;

    public DateTime? ArchivedAt { get; set; }

    public ICollection<Tenant> Tenants { get; } = [];
}
