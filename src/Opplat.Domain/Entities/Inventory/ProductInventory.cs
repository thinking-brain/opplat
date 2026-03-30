using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Inventory;

public class ProductInventory: IEntity
{
    [Key]
    [Column(Order = 1)]
    public Guid ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    [Key]
    [Column(Order = 2)]
    public Guid StorageId { get; set; }

    public virtual Storage Storage { get; set; } = null!;

    public decimal Quantity { get; set; }

    public DateTime UpdatedOn { get; set; }

    public required string User {get;set;}
}
