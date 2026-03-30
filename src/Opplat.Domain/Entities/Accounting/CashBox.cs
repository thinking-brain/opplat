namespace Opplat.Domain.Entities.Accounting;

public class CashBox : BaseEntity
{

    public required string Description { get; set; }

    public virtual ICollection<DenominationInCashBox> Cash { get; set; } = [];
}
