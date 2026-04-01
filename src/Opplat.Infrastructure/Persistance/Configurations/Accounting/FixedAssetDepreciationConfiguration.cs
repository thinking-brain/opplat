using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class FixedAssetDepreciationConfiguration : IEntityTypeConfiguration<FixedAssetDepreciation>
{
    public void Configure(EntityTypeBuilder<FixedAssetDepreciation> builder)
    {
        builder.HasOne(f => f.FixedAsset)
            .WithMany()
            .HasForeignKey(f => f.FixedAssetId);
    }
}
