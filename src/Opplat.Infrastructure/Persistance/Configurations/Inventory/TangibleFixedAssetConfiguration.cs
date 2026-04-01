using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Configurations.Inventory;

public sealed class TangibleFixedAssetConfiguration : IEntityTypeConfiguration<TangibleFixedAsset>
{
    public void Configure(EntityTypeBuilder<TangibleFixedAsset> builder)
    {
        builder.Property(t => t.Description).IsRequired();
    }
}
