using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Configurations.Inventory;

public sealed class ReleaseVoucherConfiguration : IEntityTypeConfiguration<ReleaseVoucher>
{
    public void Configure(EntityTypeBuilder<ReleaseVoucher> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasOne(r => r.Origin)
            .WithMany()
            .HasForeignKey(r => r.OriginId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Destination)
            .WithMany()
            .HasForeignKey(r => r.DestinationId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
