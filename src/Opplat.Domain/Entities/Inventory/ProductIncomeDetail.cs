
namespace Opplat.Domain.Entities.Inventory;

public class ProductIncomeDetail
{
    public Guid ProductId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}
