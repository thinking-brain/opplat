using Opplat.Domain.Dtos.Accounting;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface ICashBoxService
{
    Task<bool> Withdraw(decimal amount, string detail, string user);
    Task<bool> Deposit(decimal amount, string detail, string user);
    Task<bool> CanWithdraw(decimal amount);
    Task<IEnumerable<OperationDto>> GetOperations(DateTime date);
    Task<IEnumerable<OperationDto>> GetSales(DateTime date);
    Task<IEnumerable<CurrencyDenomination>> GetDenominations();
}
