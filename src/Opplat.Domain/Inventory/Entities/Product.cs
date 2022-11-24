using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Inventory.Entities;

public class ProductClassification: IEntity
{
    public int Id { get; set; }

    public string Description { get; set; }

    [JsonIgnore]
    public virtual ICollection<ProductGroup> Groups { get; set; }

    public ProductClassification()
    {
        Groups = new HashSet<ProductGroup>();
        Description = String.Empty;
    }

}

public class ProductGroup: IEntity
{
    public int Id { get; set; }

    public string Description { get; set; } = String.Empty;

    public int ClassificationId { get; set; }

    public virtual ProductClassification Classification { get; set; } = null!;
}

public class Product : Entity
{
    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public bool Active { get; set; }

    public bool ItsInventoriable { get; set; }

    public int GroupId { get; set; }

    public virtual ProductGroup Group { get; set; }

    public override string ToString() => Name;

    public string Unit { get; set; }

    public decimal ContainerQuantity { get; set; }

    public decimal ShrinkRatio { get; set; }

    public decimal Cost { get; set; }

    public decimal FixedValue { get; set; }
}
