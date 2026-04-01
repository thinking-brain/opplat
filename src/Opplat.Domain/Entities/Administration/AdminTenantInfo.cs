namespace Opplat.Domain.Entities.Administration;

public sealed class AdminTenantInfo : BaseEntity
{
    public string Identifier { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public string DatabaseSchema { get; set; } = string.Empty;

    public int UserCount { get; set; }

    public bool IsActive { get; set; } = true;
}
