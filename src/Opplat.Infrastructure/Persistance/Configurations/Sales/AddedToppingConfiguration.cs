using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Configurations.Sales;

public sealed class AddedToppingConfiguration : IEntityTypeConfiguration<AddedTopping>
{
    public void Configure(EntityTypeBuilder<AddedTopping> builder)
    {
        builder.HasKey(a => new { a.ToppingId, a.SaleDetailId });
        builder.HasOne(a => a.Topping)
            .WithMany()
            .HasForeignKey(a => a.ToppingId);
        builder.HasOne(a => a.SaleDetail)
            .WithMany(s => s.Toppings)
            .HasForeignKey(a => a.SaleDetailId);
    }
}
