namespace Opplat.Domain.Entities.Sales;

public class AddedTopping
{
    public Guid ToppingId { get; set; }
    public Topping? Topping { get; set; }
    public Guid SaleDetailId { get; set; }
    public SaleDetail? SaleDetail { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

}

public class Topping : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<ProductForSale> OnProducts { get; set; } = [];
}
