using Opplat.Modules.Inventory.Domain.Entities;

namespace Opplat.Services.Inventory.Api.Dtos;

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
