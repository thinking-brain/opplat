using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContabilidadWebApi.DbConfigurations
{
    public class NivelConfig : IEntityTypeConfiguration<Nivel>
    {
        public void Configure(EntityTypeBuilder<Nivel> builder)
        {
            builder.HasOne(n => n.NivelSuperior).WithMany(n => n.NivelesInferiores).IsRequired(false);
        }
    }
}