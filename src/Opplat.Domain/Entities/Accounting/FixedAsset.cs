namespace Opplat.Domain.Entities.Accounting;

public class FixedAsset : BaseEntity
{
    public required string Description { get; set; }

    public DateTime EntryDate { get; set; }

    public decimal InitialValue { get; set; }

    public decimal CurrentValue { get; set; }

    public bool IsDecommissioned { get; set; }

    public DateTime? DecommissionDate { get; set; }
}

