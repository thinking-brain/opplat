using Opplat.Shared.Entities;

namespace Opplat.Modules.Inventory.Domain.Entities;

public class TangibleFixedAsset : Entity
{
    public string Description { get; set; }

    public DateTime EntryDate { get; set; }

    public decimal InitialValue { get; set; }

    public decimal CurrentValue { get; set; }

    public bool Discharged { get; set; }

    public DateTime? DischargeDate { get; set; }
}