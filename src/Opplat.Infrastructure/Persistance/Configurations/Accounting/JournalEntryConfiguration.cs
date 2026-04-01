using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.Property(j => j.Detail).IsRequired();
        builder.HasOne(j => j.AccountingDay)
            .WithMany(a => a.JournalEntries)
            .HasForeignKey(j => j.AccountingDayId);
        builder.Ignore(j => j.IsValid);
    }
}
