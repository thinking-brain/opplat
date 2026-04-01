using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class AccountingDayConfiguration : IEntityTypeConfiguration<AccountingDay>
{
    public void Configure(EntityTypeBuilder<AccountingDay> builder)
    {
        builder.ToTable("contb_dia_contable");
    }
}
