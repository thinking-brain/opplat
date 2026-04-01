using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class CurrencyDenominationConfiguration : IEntityTypeConfiguration<CurrencyDenomination>
{
    public void Configure(EntityTypeBuilder<CurrencyDenomination> builder)
    {
        builder.Property(c => c.Description).IsRequired();
        builder.HasOne(c => c.Currency)
            .WithMany()
            .HasForeignKey(c => c.CurrencyId);
    }
}
