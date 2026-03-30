namespace Opplat.Domain.Entities.Inventory;

public class Storage: BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCostCenter { get; set; }
}
