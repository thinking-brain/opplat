using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.HasKey(m => new { m.JournalEntryId, m.AccountId });
        builder.Property(m => m.OperationType).HasConversion<string>();
        builder.HasOne(m => m.JournalEntry)
            .WithMany(j => j.Movements)
            .HasForeignKey(m => m.JournalEntryId);
        builder.HasOne(m => m.Account)
            .WithMany(l => l.Movements)
            .HasForeignKey(m => m.AccountId);
    }
}
