using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(o => o.SaleDetailId);
        builder.Property(o => o.Observations).IsRequired();
        builder.Property(o => o.CustomerType).HasConversion<string>();
        builder.HasOne(o => o.SaleDetail)
            .WithOne(s => s.OrderDetail)
            .HasForeignKey<OrderDetail>(o => o.SaleDetailId);
    }
}
