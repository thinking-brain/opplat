using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

public class DenominationInCashBox
{
    [Key]
    [Column(Order = 1)]
    public Guid CashBoxId { get; set; }

    public virtual CashBox? CashBox { get; set; }

    [Key]
    [Column(Order = 2)]
    public Guid DenominationId { get; set; }

    public virtual CurrencyDenomination? Denomination { get; set; }

    public int Quantity { get; set; }
}
