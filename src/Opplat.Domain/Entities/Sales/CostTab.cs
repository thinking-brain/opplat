using Opplat.Domain.Entities.Inventory;

namespace Opplat.Domain.Entities.Sales;

public class CostTab : IEntity
{
    public Guid ProductId { get; set; }

    public ProductForSale? Product { get; set; }

    public required string Preparation { get; set; }

    public required string Presentation { get; set; }

    public decimal PlannedCost { get; set; }

    public decimal ExpectedRate { get; set; }

    public decimal FixedCost { get; set; }

    public decimal DetailsCost => Details.Sum(d => d.Cost);

    public decimal Cost => FixedCost + DetailsCost;

    public ICollection<CostTabDetail> Details { get; set; } = [];
}

public class CostTabDetail
{
    public Guid ProductForSaleId { get; set; }

    public ProductForSale? ProductForSale { get; set; }

    public Guid ProductId { get; set; }

    public Product? Product { get; set; }

    public decimal Quantity { get; set; }

    public required string Unit { get; set; }

    public decimal FixedCost { get; set; }

    public decimal VariableCost { get; set; }

    public decimal Cost => FixedCost + VariableCost;
}