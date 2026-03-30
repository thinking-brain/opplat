
namespace Opplat.Domain.Entities;

public enum UnitType
{
    Weight = 1,
    Capacity = 2,
    Distance = 3,
    Quantity = 4,
}

public class UnitOfMeasurement
{
    public required string Abbreviation { get; set; }

    public required string Name { get; set; }

    public UnitType UnitType { get; set; }

    public decimal CovertionFactor { get; set; }

    public override string ToString() => Name;
}
