using System;
using System.Collections.Generic;
using System.Text;

namespace notificationsWebApi.Models
{
    public class UserNotification
    {
        public string UsuarioId { get; set; }
        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
        public bool IsRead { get; set; }
    }
}
