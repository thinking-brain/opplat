namespace Opplat.Domain.Entities.Administration;

public enum DatabaseInstanceStatus
{
    Active,
    Archived
}

public sealed class DatabaseInstance : BaseEntity
{

    public string Identifier { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public int CurrentTenantSchemaCount { get; set; } = 0;

    public DatabaseInstanceStatus Status { get; set; } = DatabaseInstanceStatus.Active;

    public DateTime? ArchivedAt { get; set; }

    public ICollection<Tenant> Tenants { get; } = [];
}
