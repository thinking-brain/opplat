namespace Opplat.Domain.Entities.Inventory;

public class TangibleFixedAsset : BaseEntity
{
    public required string Description { get; set; }

    public DateTime EntryDate { get; set; }

    public decimal InitialValue { get; set; }

    public decimal CurrentValue { get; set; }

    public bool Discharged { get; set; }

    public DateTime? DischargeDate { get; set; }
}