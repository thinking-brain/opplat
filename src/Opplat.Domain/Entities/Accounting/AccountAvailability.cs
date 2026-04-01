namespace Opplat.Domain.Entities.Accounting;

public class AccountAvailability
{
    public Guid AccountId { get; set; }

    public virtual LedgerAccount? Account { get; set; }

    public DateTime Date { get; set; }

    public decimal Balance { get; set; }
}
