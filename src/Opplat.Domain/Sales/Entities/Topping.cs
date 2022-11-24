using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Sales.Entities;

public class AddedTopping
{
    public Guid ToppingId { get; set; }
    public Topping Topping { get; set; }
    public Guid SaleDetailId { get; set; }
    public SaleDetail SaleDetail { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

}

public class Topping : Entity
{
    [Required]
    public string Name { get; set; }

    public ICollection<ProductForSale> OnProducts { get; set; }

    public Topping()
    {
        OnProducts = new HashSet<ProductForSale>();
    }
}
