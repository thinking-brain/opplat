using System.Data;
using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Services;

public sealed class InvoiceIssuingService(
    OpplatDbContext dbContext,
    IInvoiceCounterService invoiceCounterService,
    IInvoiceFiscalizationProvider fiscalizationProvider,
    IInvoiceTypeResolver invoiceTypeResolver,
    IInvoiceSigningService? invoiceSigningService = null)
    : IInvoiceIssuingService
{
    public async Task<InvoiceIssueResult> IssueAsync(
        Invoice invoice,
        TenantFiscalSettings settings,
        CancellationToken cancellationToken = default)
    {
        if (invoice.Status is InvoiceStatus.Issued or InvoiceStatus.Sent)
        {
            return new InvoiceIssueResult(false, "Invoice already issued.");
        }

        if (invoice.Status == InvoiceStatus.Cancelled)
        {
            return new InvoiceIssueResult(false, "Invoice is cancelled.");
        }

        var relational = dbContext.Database.IsRelational();
        var ownsTransaction = relational && dbContext.Database.CurrentTransaction is null;
        var transaction = ownsTransaction
            ? await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)
            : null;

        try
        {
            // The hash chain is per issuer, so concurrent issuing must be serialised for the whole transaction.
            if (relational && dbContext.Database.ProviderName!.Contains("Npgsql", StringComparison.OrdinalIgnoreCase))
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    "SELECT pg_advisory_xact_lock(hashtext({0}))",
                    [$"fiscal-chain:{settings.TaxId.Trim()}"],
                    cancellationToken);
            }

            var issueDate = invoice.IssueDate == default ? DateTime.UtcNow : invoice.IssueDate;
            invoice.IssueDate = issueDate;
            var fiscalYear = FiscalTime.ToFiscalDate(issueDate).Year;

            var counter = await invoiceCounterService.GetNextAsync(invoice.Series, fiscalYear, cancellationToken);
            invoice.Number = counter.LastNumber;
            invoice.FiscalYear = fiscalYear;
            invoice.FullNumber = $"{invoice.Series}-{counter.LastNumber:D6}";
            invoice.InvoiceType = invoiceTypeResolver.Resolve(settings, invoice);

            var previousRecord = await dbContext.InvoiceFiscalRecords
                .OrderByDescending(item => item.ChainSequence)
                .ThenByDescending(item => item.GeneratedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

            var record = await fiscalizationProvider.GenerateRecordAsync(invoice, settings, previousRecord, cancellationToken);
            if (record.SubmissionMode == FiscalSubmissionMode.NonVerifactuSigned)
            {
                if (invoiceSigningService is null)
                {
                    return new InvoiceIssueResult(false, "An XAdES signing service is required for signed fiscal records.");
                }

                await invoiceSigningService.SignAsync(record, cancellationToken);
            }

            invoice.FiscalRecord = record;
            invoice.Status = InvoiceStatus.Issued;
            dbContext.InvoiceFiscalRecords.Add(record);

            await dbContext.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return new InvoiceIssueResult(true, "Invoice issued.");
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
