using System.Security.Cryptography;
using System.Text;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing;

public sealed class NullInvoiceFiscalizationProvider : IInvoiceFiscalizationProvider
{
    public Task<InvoiceFiscalRecord> GenerateRecordAsync(
        Invoice invoice,
        InvoiceFiscalRecord? previousRecord = null,
        CancellationToken cancellationToken = default)
    {
        var generatedAtUtc = invoice.IssueDate == default ? DateTime.UtcNow : invoice.IssueDate;
        var input = string.Join('|',
            invoice.Series,
            invoice.Number,
            invoice.IssueDate.ToString("O"),
            invoice.InvoiceType,
            invoice.TotalAmount.ToString("0.00"),
            previousRecord?.RecordHash ?? string.Empty,
            generatedAtUtc.ToString("O"));

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input))).ToLowerInvariant();

        var record = new InvoiceFiscalRecord
        {
            InvoiceId = invoice.Id,
            PreviousRecordHash = previousRecord?.RecordHash,
            RecordHash = hash,
            HashInput = input,
            GeneratedAtUtc = generatedAtUtc,
            SoftwareName = "Opplat",
            SoftwareVersion = "1.0",
            SoftwareLicenseId = null,
            SubmissionMode = FiscalSubmissionMode.None,
            SubmissionStatus = FiscalSubmissionStatus.NotApplicable,
        };

        record.QrCodePayload = BuildQrPayload(invoice, record);
        return Task.FromResult(record);
    }

    public string BuildQrPayload(Invoice invoice, InvoiceFiscalRecord record)
        => string.Join('|', invoice.Series, invoice.Number, invoice.IssueDate.ToString("O"), invoice.TotalAmount.ToString("0.00"));
}