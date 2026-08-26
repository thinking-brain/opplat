using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class TenantPaymentMethodConfiguration : IEntityTypeConfiguration<TenantPaymentMethod>
{
    public void Configure(EntityTypeBuilder<TenantPaymentMethod> builder)
    {
        builder.ToTable("tenant_payment_methods");
        builder.Property(paymentMethod => paymentMethod.StripePaymentMethodId).HasMaxLength(128).IsRequired();
        builder.Property(paymentMethod => paymentMethod.Brand).HasMaxLength(32);
        builder.Property(paymentMethod => paymentMethod.Last4).HasMaxLength(4);
        builder.HasIndex(paymentMethod => paymentMethod.StripePaymentMethodId).IsUnique();
        builder.HasIndex(paymentMethod => new { paymentMethod.TenantId, paymentMethod.IsDefault });
    }
}
