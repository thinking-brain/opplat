using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class DenominationInCashBoxConfiguration : IEntityTypeConfiguration<DenominationInCashBox>
{
    public void Configure(EntityTypeBuilder<DenominationInCashBox> builder)
    {
        builder.HasKey(d => new { d.CashBoxId, d.DenominationId });
        builder.HasOne(d => d.CashBox)
            .WithMany(c => c.Cash)
            .HasForeignKey(d => d.CashBoxId);
        builder.HasOne(d => d.Denomination)
            .WithMany()
            .HasForeignKey(d => d.DenominationId);
    }
}
