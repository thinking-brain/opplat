using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing.Verifactu;

/// <summary>
/// <see cref="IInvoiceFiscalizationProvider"/> for tenants in
/// <see cref="InvoicingMode.Verifactu"/> mode.
/// Computes the Verifactu hash chain, builds the AEAT QR payload, and marks each
/// record as <see cref="FiscalSubmissionStatus.Pending"/> so the
/// <see cref="VerifactuSubmissionWorker"/> outbox picks it up asynchronously.
/// </summary>
public sealed class VerifactuFiscalizationProvider : IInvoiceFiscalizationProvider
{
    public Task<InvoiceFiscalRecord> GenerateRecordAsync(
        Invoice invoice,
        InvoiceFiscalRecord? previousRecord = null,
        CancellationToken cancellationToken = default)
    {
        var generatedAtUtc = DateTime.UtcNow;

        // Derive issuer NIF from the invoice customer snapshot (full invoices carry it)
        // or fall back to an empty string; the IssueInvoiceCommand handler resolves the
        // tenant NIF from TenantFiscalSettings and should embed it in the invoice before
        // calling this provider.
        var issuerNif = invoice.CustomerSnapshot?.TaxId ?? string.Empty;

        var totalTax = invoice.TaxBreakdowns.Sum(b => b.TaxAmount);

        var (hashInput, hash) = VerifactuHashCalculator.Compute(
            issuerNif,
            invoice,
            totalTax,
            generatedAtUtc,
            previousRecord?.RecordHash ?? string.Empty);

        var record = new InvoiceFiscalRecord
        {
            InvoiceId = invoice.Id,
            PreviousRecordHash = previousRecord?.RecordHash,
            RecordHash = hash,
            HashInput = hashInput,
            GeneratedAtUtc = generatedAtUtc,
            SoftwareName = "Opplat",
            SoftwareVersion = "1.0",
            SoftwareLicenseId = null,
            SubmissionMode = FiscalSubmissionMode.Verifactu,
            SubmissionStatus = FiscalSubmissionStatus.Pending,
        };

        record.QrCodePayload = BuildQrPayload(invoice, record);
        return Task.FromResult(record);
    }

    public string BuildQrPayload(Invoice invoice, InvoiceFiscalRecord record)
    {
        var issuerNif = invoice.CustomerSnapshot?.TaxId ?? string.Empty;
        return VerifactuQrCodeBuilder.BuildPayload(issuerNif, invoice, record);
    }
}
