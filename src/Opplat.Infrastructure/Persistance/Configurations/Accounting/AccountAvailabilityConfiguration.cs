using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class AccountAvailabilityConfiguration : IEntityTypeConfiguration<AccountAvailability>
{
    public void Configure(EntityTypeBuilder<AccountAvailability> builder)
    {
        builder.HasKey(a => a.AccountId);
        builder.HasOne(a => a.Account)
            .WithOne(l => l.Availability)
            .HasForeignKey<AccountAvailability>(a => a.AccountId);
    }
}
