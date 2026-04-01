using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Models;

namespace Opplat.Infrastructure.Persistance.Configurations.Core;

public sealed class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.HasKey(un => new { un.NotificationId, un.UserId });
        builder.HasOne(un => un.Notification)
            .WithMany(n => n.UserNotification)
            .HasForeignKey(un => un.NotificationId);
    }
}
