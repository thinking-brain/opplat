using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Hubs;

/// <summary>
/// Hub que se encarga de las Notificaciones
/// </summary>
[DisableCors]
public class NotificationsHub : Hub
{
    private readonly OpplatDbContext _context;
    /// <summary>
    /// Constructor del Hub
    /// </summary>
    /// <param name="context">Contexto para buscar las Notificaciones</param>
    public NotificationsHub(OpplatDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtener todas las Notificaciones de un Usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task GetUserNotifications(string userId)
    {
        var notifications = await _context.Set<UserNotification>().Include(u => u.Notification)
        .Where(u => u.UsuarioId == userId && u.IsRead == false).Select(n => new { date = n.Notification.CreateDate, text = n.Notification.Texto, created = n.Notification.Created, link = n.Notification.Link, info = n.Notification.Id }).OrderByDescending(n => n.date).ToListAsync();
        string name = Context.User!.Identity!.Name!;
        await Clients.Group(name).SendAsync("GetNotifications", notifications);
    }

    public async Task Show(string userId, Notification notificaion)
    {
        string name = Context.User!.Identity!.Name!;
        var notification = new Notification { Texto = "Hola", Link = "#" };

        await Clients.Group(name).SendAsync("ShowNotification", notification);
    }

    public async Task UserNotificationViewed(string userId, int id)
    {
        var usernotification = await _context.Set<UserNotification>().FirstOrDefaultAsync(n => n.NotificationId == id && n.UsuarioId == userId);
        if (usernotification != null)
        {
            usernotification.IsRead = true;
            _context.Update(usernotification);
            await _context.SaveChangesAsync();
        }
        var notifications = await _context.Set<UserNotification>().Include(u => u.Notification)
            .Where(u => u.UsuarioId == userId && u.IsRead == false)
            .Select(n => new
            {
                date = n.Notification.CreateDate,
                text = n.Notification.Texto,
                created = n.Notification.Created,
                link = n.Notification.Link,
                info = n.NotificationId
            }).OrderByDescending(n => n.date).ToListAsync();

        string name = Context.User!.Identity!.Name!;

        await Clients.Group(name).SendAsync("GetNotifications", notifications);
    }

    [DisableCors]
    public override Task OnConnectedAsync()
    {
        string name = "";
        try
        {
            name = Context.User!.Identity!.Name!;
        }
        catch (Exception)
        {
            name = "webmaster";
        }

        // Groups.AddToGroupAsync(Context.ConnectionId, name);

        return base.OnConnectedAsync();
    }
}
