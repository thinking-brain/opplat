using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Sales.Entities;


public enum CustomerType
{
    Undefined,
    Girl,
    Boy,
    Woman,
    Man
}

public class OrderDetail
{
    [Key]
    public Guid SaleDetailId { get; set; }

    public SaleDetail SaleDetail { get; set; }

    public int Position { get; set; }

    public CustomerType CustomerType { get; set; }

    public string Observations { get; set; }

    public virtual ICollection<Annotation> Annotations { get; set; }

    public OrderDetail()
    {
        Annotations = new HashSet<Annotation>();
    }
}

public class Annotation : Entity
{
    public string Acronym { get; set; }

    public string Description { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    public Annotation()
    {
        OrderDetails = new HashSet<OrderDetail>();
    }
}


public class SaleDetail : Entity
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; }

    public ICollection<AddedTopping> Toppings { get; set; }

    public Guid SaleId { get; set; }

    public Sale Sale { get; set; }

    public int Quatity { get; set; }

    public decimal Amount { get; set; }

    public OrderDetail OrderDetail { get; set; }
}
