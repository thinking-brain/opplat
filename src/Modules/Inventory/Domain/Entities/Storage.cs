using Opplat.Shared.Entities;

namespace Opplat.Modules.Inventory.Domain.Entities;

public class Storage: Entity
{
    public string Code { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public bool IsCostCenter { get; set; }
}
