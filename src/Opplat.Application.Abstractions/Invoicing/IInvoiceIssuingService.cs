using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

public sealed record InvoiceIssueResult(bool Succeeded, string Message);

/// <summary>
/// Assigns the number, builds the fiscal record and persists an invoice as one atomic unit.
/// Shared by the back-office "issue" command and the POS sale completion.
/// </summary>
public interface IInvoiceIssuingService
{
    /// <summary>
    /// Issues a tracked draft <paramref name="invoice"/> (lines and tax breakdowns already populated).
    /// When the caller already opened a transaction, issuing joins it and the caller commits.
    /// </summary>
    Task<InvoiceIssueResult> IssueAsync(
        Invoice invoice,
        TenantFiscalSettings settings,
        CancellationToken cancellationToken = default);
}
