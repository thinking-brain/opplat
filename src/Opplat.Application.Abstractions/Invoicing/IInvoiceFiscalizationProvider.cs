using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

public interface IInvoiceFiscalizationProvider
{
    Task<InvoiceFiscalRecord> GenerateRecordAsync(
        Invoice invoice,
        TenantFiscalSettings settings,
        InvoiceFiscalRecord? previousRecord = null,
        CancellationToken cancellationToken = default);

    string BuildQrPayload(Invoice invoice, InvoiceFiscalRecord record);
}