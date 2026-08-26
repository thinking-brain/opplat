using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models;

namespace Opplat.Application.Services;

public sealed class SubscriptionInvoiceEmailService(IOptions<SmtpSettings> smtpOptions)
    : ISubscriptionInvoiceEmailService
{
    private readonly SmtpSettings _smtp = smtpOptions.Value;

    public async Task SendAsync(
        SubscriptionInvoice invoice,
        byte[] pdfBytes,
        string recipientEmail,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.SenderName, _smtp.SenderEmail));
        message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
        message.Subject = $"Subscription invoice {invoice.StripeInvoiceId}";

        var builder = new BodyBuilder
        {
            HtmlBody = $"<p>Your subscription payment invoice <strong>{invoice.StripeInvoiceId}</strong> is attached.</p>"
        };
        builder.Attachments.Add(
            $"SubscriptionInvoice_{invoice.StripeInvoiceId}.pdf",
            pdfBytes,
            new ContentType("application", "pdf"));
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        client.ServerCertificateValidationCallback = (_, _, _, _) => true;
        await client.ConnectAsync(_smtp.Server, _smtp.Port, cancellationToken: cancellationToken);
        await client.AuthenticateAsync(_smtp.UserName, _smtp.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}