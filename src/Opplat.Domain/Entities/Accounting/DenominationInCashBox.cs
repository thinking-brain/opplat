namespace Opplat.Domain.Entities.Accounting;

public class DenominationInCashBox
{
    public Guid CashBoxId { get; set; }

    public virtual CashBox? CashBox { get; set; }

    public Guid DenominationId { get; set; }

    public virtual CurrencyDenomination? Denomination { get; set; }

    public int Quantity { get; set; }
}
