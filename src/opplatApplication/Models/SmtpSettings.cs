namespace opplatApplication.Models;

public class SmtpSettings
{
    public string Server { get; set; } = String.Empty;
    public int Port { get; set; }
    public string SenderName { get; set; } = String.Empty;
    public string SenderEmail { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}
