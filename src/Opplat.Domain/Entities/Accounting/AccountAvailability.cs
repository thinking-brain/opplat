using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Accounting;

public class AccountAvailability
{
    [Key]
    public Guid AccountId { get; set; }

    public virtual LedgerAccount? Account { get; set; }

    public DateTime Date { get; set; }

    public decimal Balance { get; set; }
}
