using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

public class DenominationsInCashBoxClosing
{
    [Key]
    [Column(Order = 1)]
    public Guid CashBoxClosingId { get; set; }

    public virtual CashBoxClosing? CashBoxClosing { get; set; }

    [Key]
    [Column(Order = 2)]
    public Guid CurrencyDenominationId { get; set; }

    public virtual CurrencyDenomination? CurrencyDenomination { get; set; }

    public int Quantity { get; set; }
}
