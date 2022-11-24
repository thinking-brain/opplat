using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Inventory.Entities;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Sales.Entities;

public class CostTab : IEntity
{
    [Key]
    public Guid ProductId { get; set; }

    public ProductForSale Product { get; set; }

    public string Preparation { get; set; }

    public string Presentation { get; set; }

    public decimal PlannedCost { get; set; }

    public decimal ExpectedRate { get; set; }

    public decimal FixedCost { get; set; }

    public decimal DetailsCost => Details.Sum(d => d.Cost);

    public decimal Cost => FixedCost + DetailsCost;

    public ICollection<CostTabDetail> Details { get; set; }

    public CostTab()
    {
        Details = new HashSet<CostTabDetail>();
    }
}

public class CostTabDetail
{
    [Key]
    [Column(Order = 1)]
    public Guid ProductForSaleId { get; set; }

    public ProductForSale ProductForSale { get; set; }

    [Key]
    [Column(Order = 2)]
    public Guid ProductId { get; set; }

    public Product Product { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; }

    public decimal FixedCost { get; set; }

    public decimal VariableCost { get; set; }

    public decimal Cost => FixedCost + VariableCost;
}