using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Persistance.Configurations.Invoicing;

public sealed class InvoiceCounterConfiguration : IEntityTypeConfiguration<InvoiceCounter>
{
    public void Configure(EntityTypeBuilder<InvoiceCounter> builder)
    {
        builder.Property(counter => counter.Series).IsRequired();
        builder.HasIndex(counter => new { counter.Series, counter.Year }).IsUnique();
    }
}