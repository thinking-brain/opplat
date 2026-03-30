using Opplat.Domain.Dtos.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface ILedgerAccountService
{
    Task<IEnumerable<LedgerAccountDto>> GetAccounts();
    Task<IEnumerable<OperationDto>> GetAccountMovements(string accountName, DateTime date);
    Task<bool> AddOperation(Guid creditAccountId, Guid debitAccountId, decimal amount,
                DateTime date, string observations, string user);
}
