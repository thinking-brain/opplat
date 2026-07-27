using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

public interface IInvoiceFiscalizationProvider
{
    Task<InvoiceFiscalRecord> GenerateRecordAsync(
        Invoice invoice,
        InvoiceFiscalRecord? previousRecord = null,
        CancellationToken cancellationToken = default);

    string BuildQrPayload(Invoice invoice, InvoiceFiscalRecord record);
}