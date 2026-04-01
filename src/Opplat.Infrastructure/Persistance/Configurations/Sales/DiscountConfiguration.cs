using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.User).IsRequired();
        builder.HasOne(d => d.Sale)
            .WithMany(s => s.Discounts)
            .HasForeignKey(d => d.SaleId);
    }
}
