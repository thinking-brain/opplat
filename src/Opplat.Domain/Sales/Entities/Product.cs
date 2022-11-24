using System.ComponentModel.DataAnnotations;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Sales.Entities;

/// <summary>
/// ProductTag
/// </summary>
public class ProductTag: Entity
{
    [Required]
    public string Name { get; set; }

    public ICollection<ProductForSale> OnProducts { get; set; }
    public ProductTag()
    {
        OnProducts = new HashSet<ProductForSale>();
    }
}


/// <summary>
/// ProductForSale
/// </summary>
public class ProductForSale: Entity
{
    [Required]
    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public bool Active { get; set; }
    public ICollection<Topping> AvailableToppings { get; set; }
    public ICollection<ProductTag> Tags { get; set; }
    public ProductForSale()
    {
        AvailableToppings = new HashSet<Topping>();
        Tags = new HashSet<ProductTag>();
    }
}
