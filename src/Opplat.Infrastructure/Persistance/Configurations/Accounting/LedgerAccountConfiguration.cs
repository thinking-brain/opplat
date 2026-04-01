using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class LedgerAccountConfiguration : IEntityTypeConfiguration<LedgerAccount>
{
    public void Configure(EntityTypeBuilder<LedgerAccount> builder)
    {
        builder.Property(l => l.Nature).HasConversion<string>();
        builder.HasOne(l => l.Level)
            .WithMany()
            .HasForeignKey(l => l.LevelId);
        builder.Ignore(l => l.Number);
        builder.Ignore(l => l.Name);
        builder.Ignore(l => l.IsValid);
    }
}
