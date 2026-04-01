using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Configurations.Inventory;

public sealed class ProductClassificationConfiguration : IEntityTypeConfiguration<ProductClassification>
{
    public void Configure(EntityTypeBuilder<ProductClassification> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn();
        builder.Property(p => p.Description).IsRequired(false);
    }
}
