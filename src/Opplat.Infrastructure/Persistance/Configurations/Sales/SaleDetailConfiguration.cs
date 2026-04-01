using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class SaleDetailConfiguration : IEntityTypeConfiguration<SaleDetail>
{
    public void Configure(EntityTypeBuilder<SaleDetail> builder)
    {
        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId);
        builder.HasOne(s => s.Sale)
            .WithMany(sale => sale.Products)
            .HasForeignKey(s => s.SaleId);
    }
}
