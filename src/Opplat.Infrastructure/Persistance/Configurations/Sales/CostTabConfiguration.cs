using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class CostTabConfiguration : IEntityTypeConfiguration<CostTab>
{
    public void Configure(EntityTypeBuilder<CostTab> builder)
    {
        builder.HasKey(c => c.ProductId);
        builder.Property(c => c.Preparation).IsRequired();
        builder.Property(c => c.Presentation).IsRequired();
        builder.HasOne(c => c.Product)
            .WithOne()
            .HasForeignKey<CostTab>(c => c.ProductId);
    }
}
