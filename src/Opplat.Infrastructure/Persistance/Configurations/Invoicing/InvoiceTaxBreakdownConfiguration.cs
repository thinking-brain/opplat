using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Persistance.Configurations.Invoicing;

public sealed class InvoiceTaxBreakdownConfiguration : IEntityTypeConfiguration<InvoiceTaxBreakdown>
{
    public void Configure(EntityTypeBuilder<InvoiceTaxBreakdown> builder)
    {
    }
}