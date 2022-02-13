using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContabilidadWebApi.DbConfigurations
{
    public class CuentaConfig : IEntityTypeConfiguration<Cuenta>
    {
        public void Configure(EntityTypeBuilder<Cuenta> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(n => n.CuentaSuperior).WithMany(n => n.Subcuentas).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        }
    }
}