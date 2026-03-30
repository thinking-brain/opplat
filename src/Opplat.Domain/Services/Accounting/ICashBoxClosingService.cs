using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface ICashBoxClosingService
{
    Task<IEnumerable<CashBoxClosing>> GetClosings();
    Task<bool> CanClose();
}
