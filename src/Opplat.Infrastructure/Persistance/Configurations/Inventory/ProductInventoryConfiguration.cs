using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Configurations.Inventory;

public sealed class ProductInventoryConfiguration : IEntityTypeConfiguration<ProductInventory>
{
    public void Configure(EntityTypeBuilder<ProductInventory> builder)
    {
        builder.HasKey(p => new { p.ProductId, p.StorageId });
        builder.Property(p => p.User).IsRequired();
        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId);
        builder.HasOne(p => p.Storage)
            .WithMany()
            .HasForeignKey(p => p.StorageId);
    }
}
