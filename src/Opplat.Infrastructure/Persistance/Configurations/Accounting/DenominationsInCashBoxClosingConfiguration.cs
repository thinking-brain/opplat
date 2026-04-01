using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class DenominationsInCashBoxClosingConfiguration : IEntityTypeConfiguration<DenominationsInCashBoxClosing>
{
    public void Configure(EntityTypeBuilder<DenominationsInCashBoxClosing> builder)
    {
        builder.HasKey(d => new { d.CashBoxClosingId, d.CurrencyDenominationId });
        builder.HasOne(d => d.CashBoxClosing)
            .WithMany(c => c.Breakdown)
            .HasForeignKey(d => d.CashBoxClosingId);
        builder.HasOne(d => d.CurrencyDenomination)
            .WithMany()
            .HasForeignKey(d => d.CurrencyDenominationId);
    }
}
