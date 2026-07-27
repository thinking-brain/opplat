using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Persistance.Configurations.Invoicing;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(invoice => invoice.Series).IsRequired();
        builder.Property(invoice => invoice.FullNumber).IsRequired();
        builder.Property(invoice => invoice.Currency).IsRequired();
        builder.HasIndex(invoice => new { invoice.Series, invoice.Number }).IsUnique();

        builder.HasOne(invoice => invoice.Customer)
            .WithMany()
            .HasForeignKey(invoice => invoice.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(invoice => invoice.Sale)
            .WithMany()
            .HasForeignKey(invoice => invoice.SaleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(invoice => invoice.Lines)
            .WithOne(line => line.Invoice)
            .HasForeignKey(line => line.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(invoice => invoice.TaxBreakdowns)
            .WithOne(taxBreakdown => taxBreakdown.Invoice)
            .HasForeignKey(taxBreakdown => taxBreakdown.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(invoice => invoice.FiscalRecord)
            .WithOne(record => record.Invoice)
            .HasForeignKey<InvoiceFiscalRecord>(record => record.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(invoice => invoice.CustomerSnapshot, snapshot =>
        {
            snapshot.Property(customerSnapshot => customerSnapshot.Name).IsRequired();
        });
    }
}