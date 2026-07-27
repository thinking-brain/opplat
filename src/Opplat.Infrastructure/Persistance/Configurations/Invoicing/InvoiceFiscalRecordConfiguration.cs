using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Persistance.Configurations.Invoicing;

public sealed class InvoiceFiscalRecordConfiguration : IEntityTypeConfiguration<InvoiceFiscalRecord>
{
    public void Configure(EntityTypeBuilder<InvoiceFiscalRecord> builder)
    {
        builder.Property(record => record.RecordHash).IsRequired();
        builder.Property(record => record.HashInput).IsRequired();
        builder.Property(record => record.SoftwareName).IsRequired();
        builder.Property(record => record.SoftwareVersion).IsRequired();
        builder.HasIndex(record => record.InvoiceId).IsUnique();
    }
}