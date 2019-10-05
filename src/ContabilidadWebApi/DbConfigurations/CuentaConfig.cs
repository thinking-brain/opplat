using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContabilidadWebApi.DbConfigurations
{
    public class CuentaConfig : IEntityTypeConfiguration<Cuenta>
    {
        public void Configure(EntityTypeBuilder<Cuenta> builder)
        {
            builder.HasOne(n => n.CuentaSuperior).WithMany(n => n.Subcuentas).IsRequired(false);
            builder.HasOne(c => c.Disponibilidad).WithOne(d => d.Cuenta).IsRequired().HasPrincipalKey<Disponibilidad>();
        }
    }
}