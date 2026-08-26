using Opplat.Domain.Entities.Administration;

namespace Opplat.Application.Abstractions.Invoicing;

public interface ISubscriptionInvoiceEmailService
{
    Task SendAsync(
        SubscriptionInvoice invoice,
        byte[] pdfBytes,
        string recipientEmail,
        CancellationToken cancellationToken = default);
}