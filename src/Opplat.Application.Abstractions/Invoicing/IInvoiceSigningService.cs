using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

/// <summary>
/// Signs a <see cref="InvoiceFiscalRecord"/> with an electronic signature (XAdES) for the
/// non-Verifactu SIF compliance path defined in RD 1007/2023.
/// </summary>
/// <remarks>
/// Certificate upload, storage, and XAdES signing are out of scope for the current MVP.
/// The only registered implementation is <c>NotImplementedInvoiceSigningService</c>, which
/// throws <see cref="NotImplementedException"/> and serves as a placeholder until a real
/// signing pipeline is built.
/// </remarks>
public interface IInvoiceSigningService
{
    /// <summary>
    /// Computes an XAdES-BES signature over the canonical XML representation of the fiscal
    /// record and writes the result into <see cref="InvoiceFiscalRecord.SignatureValue"/>.
    /// </summary>
    Task<string> SignAsync(InvoiceFiscalRecord record, CancellationToken cancellationToken = default);
}
