namespace Opplat.Domain.Models.Administration;

public sealed class BulkTenantProvisioningResult
{
    public int Total { get; init; }
    public int Succeeded { get; init; }
    public int Failed { get; init; }
    public IReadOnlyList<TenantProvisioningResult> Results { get; init; } = [];
}

public sealed class TenantProvisioningResult
{
    public required Guid TenantId { get; set; }
    public required string TenantIdentifier { get; set; }
    public string DatabaseSchema { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string DatabaseInstanceIdentifier { get; set; } = string.Empty;
    public Guid DatabaseInstanceId { get; set; }
    public bool Succeeded { get; set; }
    public bool AlreadyProvisioned { get; set; }
    public bool InstanceReassigned { get; set; }
    public bool DatabaseCreated { get; set; }
    public bool SchemaCreated { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ExecutedAt { get; set; }
}


public sealed class TenantSchemaProvisioningOutcome
{
    public required string TenantId { get; set; }
    public required string TenantIdentifier { get; set; }
    public required string DatabaseSchema { get; set; }
    public required string DatabaseName { get; set; }
    public required string DatabaseInstanceIdentifier { get; set; }
    public int DatabaseInstanceId { get; set; }
    public bool Succeeded { get; set; }
    public bool AlreadyProvisioned { get; set; }
    public bool DatabaseCreated { get; set; }
    public bool SchemaCreated { get; set; }
    public bool MigrationsApplied { get; set; }
    public DateTime ExecutedAt { get; set; }
}
