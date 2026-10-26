namespace Opplat.Domain.Entities.Sales;

/// <summary>
/// ProductTag
/// </summary>
public class ProductTag : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<ProductForSale> OnProducts { get; set; } = [];
}


/// <summary>
/// ProductForSale
/// </summary>
public class ProductForSale : BaseEntity
{
    public required string Name { get; set; }

    public required string Code { get; set; }

    public required string Description { get; set; }

    public decimal Price { get; set; }

    /// <summary>VAT percentage included in <see cref="Price"/>.</summary>
    public decimal TaxRate { get; set; } = 21m;

    public string? Barcode { get; set; }

    /// <summary>Inventory product whose stock is decremented when this item is sold.</summary>
    public Guid? InventoryProductId { get; set; }

    public bool Active { get; set; }
    public ICollection<Topping> AvailableToppings { get; set; } = [];
    public ICollection<ProductTag> Tags { get; set; } = [];
}
