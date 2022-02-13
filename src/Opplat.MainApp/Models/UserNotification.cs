namespace Opplat.MainApp.Models;


public class UserNotification
{
    public string UsuarioId { get; set; } = String.Empty;
    public int NotificationId { get; set; }
    public Notification Notification { get; set; } = null!;
    public bool IsRead { get; set; }
}

