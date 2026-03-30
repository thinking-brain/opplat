
namespace Opplat.Domain.Entities.Accounting;

public class AccountingHistory : BaseEntity
{
    public DateTime Date { get; set; }

    public required string Description { get; set; }
}
