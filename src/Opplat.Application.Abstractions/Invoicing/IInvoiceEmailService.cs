using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Application.Abstractions.Invoicing;

/// <summary>Sends an invoice PDF to a recipient by email.</summary>
public interface IInvoiceEmailService
{
    /// <summary>
    /// Sends the rendered PDF for <paramref name="invoice"/> to
    /// <paramref name="recipientEmail"/> as an attachment.
    /// </summary>
    Task SendAsync(Invoice invoice, byte[] pdfBytes, string recipientEmail, CancellationToken cancellationToken = default);
}
