using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public enum OperationType
{
    Credit,
    Debit,
}

public class Movement
{
    [Key]
    public Guid JournalEntryId { get; set; }

    public virtual JournalEntry? JournalEntry { get; set; }

    [Key]
    public Guid AccountId { get; set; }

    public virtual LedgerAccount? Account { get; set; }

    public decimal Amount { get; set; }

    public OperationType OperationType { get; set; }
}
