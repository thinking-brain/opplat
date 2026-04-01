using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class CashBoxClosingConfiguration : IEntityTypeConfiguration<CashBoxClosing>
{
    public void Configure(EntityTypeBuilder<CashBoxClosing> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasOne(c => c.AccountingDay)
            .WithMany()
            .HasForeignKey(c => c.AccountingDayId);
        builder.HasOne(c => c.CashBox)
            .WithMany()
            .HasForeignKey(c => c.CashBoxId);
    }
}
