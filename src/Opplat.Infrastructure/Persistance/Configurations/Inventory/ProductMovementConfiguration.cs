using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Configurations.Inventory;

public sealed class ProductMovementConfiguration : IEntityTypeConfiguration<ProductMovement>
{
    public void Configure(EntityTypeBuilder<ProductMovement> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Observations).IsRequired();
        builder.Property(p => p.User).IsRequired();
        builder.Property(p => p.Type).HasConversion<string>();
        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId);
        builder.HasOne(p => p.Storage)
            .WithMany()
            .HasForeignKey(p => p.StorageId);
    }
}
