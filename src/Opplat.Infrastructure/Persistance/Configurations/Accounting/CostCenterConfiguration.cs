using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Infrastructure.Persistance.Configurations.Accounting;

public sealed class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.HasKey(c => c.Id);
        builder.ToTable("contb_centros_de_costo");
        builder.Property(c => c.Code).IsRequired();
        builder.Property(c => c.Name).IsRequired();
    }
}
