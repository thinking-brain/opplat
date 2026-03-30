namespace Opplat.Domain.Entities.Accounting;

public class CashBoxClosing : BaseEntity
{

    public Guid AccountingDayId { get; set; }

    public virtual AccountingDay? AccountingDay { get; set; }

    public DateTime Date { get; set; }

    public Guid CashBoxId { get; set; }

    public virtual CashBox? CashBox { get; set; }

    public virtual ICollection<DenominationsInCashBoxClosing> Breakdown { get; set; } = [];
}
