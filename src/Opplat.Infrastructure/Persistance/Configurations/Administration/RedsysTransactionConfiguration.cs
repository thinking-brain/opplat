using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Configurations.Administration;

public sealed class RedsysTransactionConfiguration : IEntityTypeConfiguration<RedsysTransaction>
{
    public void Configure(EntityTypeBuilder<RedsysTransaction> builder)
    {
        builder.ToTable("redsys_transactions");

        builder.Property(transaction => transaction.Order).HasMaxLength(12).IsRequired();
        builder.Property(transaction => transaction.Kind).HasConversion<string>().IsRequired();
        builder.Property(transaction => transaction.Status).HasConversion<string>().IsRequired();
        builder.Property(transaction => transaction.ResponseCode).HasMaxLength(8);
        builder.Property(transaction => transaction.CofTxnId).HasMaxLength(64);
        builder.Property(transaction => transaction.IdentifierToken).HasMaxLength(128);
        builder.Property(transaction => transaction.Currency).HasMaxLength(3).IsRequired();

        builder.HasIndex(transaction => transaction.Order).IsUnique();
        builder.HasIndex(transaction => new { transaction.TenantId, transaction.Kind, transaction.Status });

        builder.HasOne(transaction => transaction.Tenant)
            .WithMany()
            .HasForeignKey(transaction => transaction.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
