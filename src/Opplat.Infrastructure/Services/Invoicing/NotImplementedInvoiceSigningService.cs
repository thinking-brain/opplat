using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing;

/// <summary>
/// Placeholder implementation of <see cref="IInvoiceSigningService"/>.
/// XAdES electronic signing is out of scope for the current MVP.
/// Replace with a real implementation once certificate management is built.
/// </summary>
public sealed class NotImplementedInvoiceSigningService : IInvoiceSigningService
{
    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public Task<string> SignAsync(InvoiceFiscalRecord record, CancellationToken cancellationToken = default)
        => throw new NotImplementedException(
            "XAdES electronic signing is not yet implemented. " +
            "Certificate upload, storage, and signing must be built as a dedicated follow-up phase.");
}
