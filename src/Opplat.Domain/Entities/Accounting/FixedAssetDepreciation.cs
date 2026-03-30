namespace Opplat.Domain.Entities.Accounting;

public class FixedAssetDepreciation : JournalEntry
{
    public int FixedAssetId { get; set; }

    public virtual FixedAsset? FixedAsset { get; set; }
}

