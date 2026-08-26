using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

/// <summary>
/// Signs a <see cref="InvoiceFiscalRecord"/> with an electronic signature (XAdES) for the
/// non-Verifactu SIF compliance path defined in RD 1007/2023.
/// </summary>
/// <remarks>The certificate is supplied to the infrastructure implementation by configuration.</remarks>
public interface IInvoiceSigningService
{
    /// <summary>
    /// Computes an XAdES-BES signature over the canonical XML representation of the fiscal
    /// record and writes the result into <see cref="InvoiceFiscalRecord.SignatureValue"/>.
    /// </summary>
    Task<string> SignAsync(InvoiceFiscalRecord record, CancellationToken cancellationToken = default);
}
