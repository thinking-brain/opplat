using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class SubscriptionInvoiceConfiguration : IEntityTypeConfiguration<SubscriptionInvoice>
{
    public void Configure(EntityTypeBuilder<SubscriptionInvoice> builder)
    {
        builder.ToTable("subscription_invoices");
        builder.Property(invoice => invoice.StripeInvoiceId).HasMaxLength(128).IsRequired();
        builder.Property(invoice => invoice.Currency).HasMaxLength(3).IsRequired();
        builder.Property(invoice => invoice.Status).HasConversion<string>().IsRequired();
        builder.Property(invoice => invoice.HostedInvoiceUrl).HasMaxLength(2048);
        builder.HasIndex(invoice => invoice.StripeInvoiceId).IsUnique();
        builder.HasIndex(invoice => new { invoice.TenantId, invoice.PeriodEnd });
    }
}
