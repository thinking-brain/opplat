using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class CostTabDetailConfiguration : IEntityTypeConfiguration<CostTabDetail>
{
    public void Configure(EntityTypeBuilder<CostTabDetail> builder)
    {
        builder.HasKey(c => new { c.ProductForSaleId, c.ProductId });
        builder.Property(c => c.Unit).IsRequired();
    }
}
