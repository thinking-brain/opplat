using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class ProductForSaleConfiguration : IEntityTypeConfiguration<ProductForSale>
{
    public void Configure(EntityTypeBuilder<ProductForSale> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Code).IsRequired();
        builder.Property(p => p.Description).IsRequired();
    }
}
