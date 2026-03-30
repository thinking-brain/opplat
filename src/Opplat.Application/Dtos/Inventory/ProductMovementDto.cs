using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Dtos.Inventory;

public class ProductMovementDto
{
    public DateTime? Date { get; set; }

    public Guid ProductId { get; set; }

    public Guid StorageId { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public MovementType Type { get; set; }

    public string Observations { get; set; } = string.Empty;
}
