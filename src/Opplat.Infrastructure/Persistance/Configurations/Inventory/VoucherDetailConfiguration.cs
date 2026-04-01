using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Configurations.Inventory;

public sealed class VoucherDetailConfiguration : IEntityTypeConfiguration<VoucherDetail>
{
    public void Configure(EntityTypeBuilder<VoucherDetail> builder)
    {
        builder.HasKey(v => new { v.ProductId, v.VoucherId });
        builder.HasOne(v => v.Product)
            .WithMany()
            .HasForeignKey(v => v.ProductId);
        builder.HasOne(v => v.Voucher)
            .WithMany(r => r.Products)
            .HasForeignKey(v => v.VoucherId);
    }
}
