using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContabilidadWebApi.DbConfigurations
{
    public class AsientoConfig : IEntityTypeConfiguration<Asiento>
    {
        public void Configure(EntityTypeBuilder<Asiento> builder)
        {
            builder.HasOne(d => d.DiaContable).WithMany(a => a.Asientos).IsRequired().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
