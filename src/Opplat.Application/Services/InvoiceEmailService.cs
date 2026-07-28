using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Domain.Models;

namespace Opplat.Application.Services;

/// <summary>Sends invoice PDFs by email using MailKit / SMTP.</summary>
public sealed class InvoiceEmailService(IOptions<SmtpSettings> smtpOptions) : IInvoiceEmailService
{
    private readonly SmtpSettings _smtp = smtpOptions.Value;

    public async Task SendAsync(
        Invoice invoice,
        byte[] pdfBytes,
        string recipientEmail,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.SenderName, _smtp.SenderEmail));
        message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
        message.Subject = $"Factura {invoice.FullNumber}";

        var builder = new BodyBuilder
        {
            HtmlBody = $"<p>Adjunta encontrará la factura <strong>{invoice.FullNumber}</strong> con fecha {invoice.IssueDate:dd/MM/yyyy}.</p>"
        };
        builder.Attachments.Add($"Factura_{invoice.FullNumber}.pdf", pdfBytes, new ContentType("application", "pdf"));
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        client.ServerCertificateValidationCallback = (_, _, _, _) => true;
        await client.ConnectAsync(_smtp.Server, _smtp.Port, cancellationToken: cancellationToken);
        await client.AuthenticateAsync(_smtp.UserName, _smtp.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}
