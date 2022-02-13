using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using opplatApplication.Models;

namespace opplatApplication.Services;

public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;
    public EmailSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }
    public async Task SendEmailAsync(string emails, string subject, string htmlMessage)
    {
        if (emails != null)
        {
            var email_list = emails.Trim().Split(' ');

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            foreach (var item in email_list)
            {
                message.To.Add(new MailboxAddress(item,item));
            }
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };
            var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port);
                await client.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (System.Exception e)
            {

                await Task.FromException(e);
            }
        }
        await Task.CompletedTask;
    }
}
