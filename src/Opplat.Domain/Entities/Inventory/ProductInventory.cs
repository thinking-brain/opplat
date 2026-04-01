namespace Opplat.Domain.Entities.Inventory;

public class ProductInventory: IEntity
{
    public Guid ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public Guid StorageId { get; set; }

    public virtual Storage Storage { get; set; } = null!;

    public decimal Quantity { get; set; }

    public DateTime UpdatedOn { get; set; }

    public required string User {get;set;}
}
