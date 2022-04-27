using Opplat.Shared.Entities;

namespace Opplat.Domain.Inventory.Entities;

public class Storage: Entity
{
    public string Code { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public bool IsCostCenter { get; set; }
}
