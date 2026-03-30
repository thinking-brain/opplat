
namespace Opplat.Domain.Entities.Inventory;

public enum MovementType
{
    Input = 1,
    ToProduction = 2,
    IndependentSale = 3,
    Waste = 4,
    OutputInternalTranfer = 5,
    InputInternalTranfer = 6,
    InputForErrorInOutput = 7,
    OutputForErrorInInput = 8,
    InputByAdjustment = 9,
    OutpuByAdjustment = 10,
    OutputProductConversion = 11,
    InputProductConvertion = 12,
}

public class ProductMovement : BaseEntity
{
    public DateTime Date { get; set; }

    public Guid ProductId { get; set; }

    public Product? Product { get; set; }

    public decimal Quantity { get; set; }

    public decimal Cost { get; set; }

    public Guid StorageId { get; set; }

    public Storage? Storage { get; set; }

    public MovementType Type { get; set; }

    public required string Observations { get; set; }

    public required string User { get; set; }
}
