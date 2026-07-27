using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

public interface IInvoiceCounterService
{
    Task<InvoiceCounter> GetNextAsync(string series, int year, CancellationToken cancellationToken = default);
}