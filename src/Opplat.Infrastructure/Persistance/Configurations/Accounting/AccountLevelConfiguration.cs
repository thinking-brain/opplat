using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class AccountLevelConfiguration : IEntityTypeConfiguration<AccountLevel>
{
    public void Configure(EntityTypeBuilder<AccountLevel> builder)
    {
        builder.Property(a => a.Number).IsRequired();
        builder.Property(a => a.Name).IsRequired();
        builder.HasOne(a => a.ParentLevel)
            .WithMany(a => a.ChildLevels)
            .HasForeignKey(a => a.ParentLevelId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
