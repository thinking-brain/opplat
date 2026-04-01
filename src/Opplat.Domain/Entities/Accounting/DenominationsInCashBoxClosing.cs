namespace Opplat.Domain.Entities.Accounting;

public class DenominationsInCashBoxClosing
{
    public Guid CashBoxClosingId { get; set; }

    public virtual CashBoxClosing? CashBoxClosing { get; set; }

    public Guid CurrencyDenominationId { get; set; }

    public virtual CurrencyDenomination? CurrencyDenomination { get; set; }

    public int Quantity { get; set; }
}
