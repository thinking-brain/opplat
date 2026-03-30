namespace Opplat.Domain.Models;


public class UserNotification
{
    public string UsuarioId { get; set; } = string.Empty;
    public int NotificationId { get; set; }
    public Notification Notification { get; set; } = null!;
    public bool IsRead { get; set; }
}

