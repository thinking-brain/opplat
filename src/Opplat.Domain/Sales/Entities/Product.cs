
using Opplat.Shared.Entities;

namespace Opplat.Domain.Sales.Entities;

public class Product: Entity
{
    // [Required]
    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public bool Active { get; set; }
}
