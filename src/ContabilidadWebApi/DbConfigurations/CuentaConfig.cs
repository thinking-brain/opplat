using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContabilidadWebApi.DbConfigurations
{
    public class CuentaConfig : IEntityTypeConfiguration<Cuenta>
    {
        public void Configure(EntityTypeBuilder<Cuenta> builder)
        {
            builder.HasOne(c => c.Nivel).WithOne().IsRequired();
            builder.HasOne(c => c.Disponibilidad).WithOne(d => d.Cuenta).IsRequired().HasPrincipalKey<Disponibilidad>();
        }
    }
}