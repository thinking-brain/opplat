
namespace Opplat.Application.Dtos;

public class InventoryDto
{
    public Guid ProductId { get; set; }

    public string Product { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;
}
