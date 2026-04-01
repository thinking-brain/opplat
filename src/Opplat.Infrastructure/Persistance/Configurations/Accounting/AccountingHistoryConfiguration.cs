using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class AccountingHistoryConfiguration : IEntityTypeConfiguration<AccountingHistory>
{
    public void Configure(EntityTypeBuilder<AccountingHistory> builder)
    {
        builder.Property(a => a.Description).IsRequired();
    }
}
