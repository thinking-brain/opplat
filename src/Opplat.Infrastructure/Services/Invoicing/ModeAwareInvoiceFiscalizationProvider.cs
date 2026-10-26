using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Services.Invoicing.Verifactu;

namespace Opplat.Infrastructure.Services.Invoicing;

/// <summary>Chooses the fiscalization provider from the tenant's <see cref="InvoicingMode"/>.</summary>
public sealed class ModeAwareInvoiceFiscalizationProvider(
    NullInvoiceFiscalizationProvider nullProvider,
    VerifactuFiscalizationProvider verifactuProvider) : IInvoiceFiscalizationProvider
{
    public Task<InvoiceFiscalRecord> GenerateRecordAsync(
        Invoice invoice,
        TenantFiscalSettings settings,
        InvoiceFiscalRecord? previousRecord = null,
        CancellationToken cancellationToken = default)
        => Select(settings).GenerateRecordAsync(invoice, settings, previousRecord, cancellationToken);

    public string BuildQrPayload(Invoice invoice, InvoiceFiscalRecord record)
        => (record.SubmissionMode == FiscalSubmissionMode.Verifactu ? (IInvoiceFiscalizationProvider)verifactuProvider : nullProvider)
            .BuildQrPayload(invoice, record);

    private IInvoiceFiscalizationProvider Select(TenantFiscalSettings settings)
        => settings.InvoicingMode == InvoicingMode.Verifactu ? verifactuProvider : nullProvider;
}
