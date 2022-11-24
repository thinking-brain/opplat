
namespace Opplat.Shared.Entities;

public enum UnitType
{
    Weight = 1,
    Capacity = 2,
    Distance = 3,
    Quantity = 4,
}

public class UnitOfMeasurement
{
    public string Abbreviation { get; set; }
    
    public string Name { get; set; }

    public virtual UnitType UnitType { get; set; }

    public decimal CovertionFactor { get; set; }

    public override string ToString() => Name;
}
