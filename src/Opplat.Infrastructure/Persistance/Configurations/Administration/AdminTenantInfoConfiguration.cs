using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class AdminTenantInfoConfiguration : IEntityTypeConfiguration<AdminTenantInfo>
{
    public void Configure(EntityTypeBuilder<AdminTenantInfo> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Identifier).IsRequired();
        builder.Property(a => a.Name).IsRequired();
        builder.Property(a => a.DatabaseName).IsRequired();
        builder.Property(a => a.DatabaseSchema).IsRequired();
    }
}
