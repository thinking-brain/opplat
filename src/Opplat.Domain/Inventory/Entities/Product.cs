using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Inventory.Entities;

public class ProductClassification
{
    public int Id { get; set; }

    public string Description { get; set; }

    public virtual ICollection<ProductGroup> Groups { get; set; }

    public ProductClassification()
    {
        Groups = new HashSet<ProductGroup>();
        Description = String.Empty;
    }

}

public class ProductGroup
{
    public int Id { get; set; }

    public string Description { get; set; } = String.Empty;

    public int ClassificationId { get; set; }

    public virtual ProductClassification Classification { get; set; } = null!;
}

public class Product : Entity
{
    [Required]
    public string Name { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public bool Active { get; set; }

    public bool ItsInventoriable { get; set; }

    public int GroupId { get; set; }

    public virtual ProductGroup Group { get; set; } = null!;

    public override string ToString() => Name;

    public int UnitId { get; set; }

    public virtual UnitOfMeasurement Unit { get; set; } = null!;

    public decimal ContainerQuantity { get; set; }

    public decimal ShrinkRatio { get; set; }

    public decimal Cost { get; set; }

    public decimal FixedValue { get; set; }
}
