
namespace Opplat.Domain.Entities.Accounting;

public class CurrencyDenomination : BaseEntity
{
    public Guid CurrencyId { get; set; }

    public virtual Currency? Currency { get; set; }

    public decimal Value { get; set; }

    public required string Description { get; set; }

    public bool IsBanknote { get; set; }
}
