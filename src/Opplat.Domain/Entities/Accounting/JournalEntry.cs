namespace Opplat.Domain.Entities.Accounting;

public class JournalEntry : BaseEntity
{
    public Guid AccountingDayId { get; set; }

    public virtual AccountingDay? AccountingDay { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<Movement> Movements { get; set; } = [];

    public required string CreatedBy { get; set; }

    public required string Detail { get; set; }

    public bool IsValid
    {
        get
        {
            return Movements.Where(m => m.OperationType == OperationType.Credit).Sum(c => c.Amount) == Movements.Where(m => m.OperationType == OperationType.Debit).Sum(d => d.Amount);
        }
    }
}
