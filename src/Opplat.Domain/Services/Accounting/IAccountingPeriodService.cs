using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface IAccountingPeriodService
{
    Task<IEnumerable<AccountingDay>> GetAccountingPeriods();
    Task<AccountingDay> GetCurrentAccountingDay();
    Task<AccountingDay> FindById(Guid id);
    Task<bool> Update(AccountingDay accountingDay);
}
