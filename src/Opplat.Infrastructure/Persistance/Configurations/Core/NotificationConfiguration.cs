using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Models;

namespace Opplat.Infrastructure.Persistance.Configurations.Core;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).UseIdentityByDefaultColumn();
        builder.Property(n => n.Text).IsRequired(false);
        builder.Property(n => n.Link).IsRequired(false);
        builder.Property(n => n.Module).IsRequired(false);
        builder.Ignore(n => n.Created);
    }
}
