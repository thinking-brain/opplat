using System.ComponentModel.DataAnnotations;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Inventory.Entities;

public class UnitType: Entity
{
    [Required]
    public string Name { get; set; }

    public virtual ICollection<UnitOfMeasurement> Units { get; set; }

    public UnitType()
    {
        Units = new HashSet<UnitOfMeasurement>();
        Name = String.Empty;
    }
}

public class UnitOfMeasurement: Entity
{
    [Required]
    public string Name { get; set; } = String.Empty;

    [Required]
    public string Acronym { get; set; } = String.Empty;

    public Guid UnitTypeId { get; set; }

    public virtual UnitType UnitType { get; set; } = null!;

    public decimal CovertionFactor { get; set; }

    public override string ToString() => Name;
}
