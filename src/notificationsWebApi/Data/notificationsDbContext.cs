using notificationsWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace notificationsWebApi.Data
{
    public class notificationsDbContext : IdentityDbContext
    {
        public notificationsDbContext()
        {

        }
        public notificationsDbContext(DbContextOptions<notificationsDbContext> options) : base(options)
        {

        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

    }
}