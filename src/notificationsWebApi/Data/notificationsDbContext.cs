using notificationsWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace notificationsWebApi.Data
{
    public class notificationsDbContext : DbContext
    {
        public notificationsDbContext(DbContextOptions<notificationsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserNotification>().HasKey(s => new { s.NotificationId, s.UsuarioId });
        }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

    }
}