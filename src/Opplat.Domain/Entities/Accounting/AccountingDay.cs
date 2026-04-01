namespace Opplat.Domain.Entities.Accounting;

public class AccountingDay : BaseEntity
{
    public DateTime Date { get; set; }

    public bool IsOpen { get; set; }

    public DateTime? ClosedAt { get; set; }

    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = [];
}
