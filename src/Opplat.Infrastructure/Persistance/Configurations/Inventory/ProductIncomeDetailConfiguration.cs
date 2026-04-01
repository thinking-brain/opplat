using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Configurations.Inventory;

public sealed class ProductIncomeDetailConfiguration : IEntityTypeConfiguration<ProductIncomeDetail>
{
    public void Configure(EntityTypeBuilder<ProductIncomeDetail> builder)
    {
        builder.HasKey(p => new { p.ProductId, p.UnitId });
    }
}
