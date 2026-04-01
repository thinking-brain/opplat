using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Models;

namespace Opplat.Infrastructure.Persistance.Configurations.Core;

public sealed class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).UseIdentityByDefaultColumn();
        builder.Property(l => l.Application).IsRequired();
        builder.Property(l => l.Subscriber).IsRequired();
        builder.Property(l => l.ExpirationDate).IsRequired();
        builder.Property(l => l.Hash).IsRequired();
    }
}
