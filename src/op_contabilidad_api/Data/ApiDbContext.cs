using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using op_contabilidad_api.Models;

namespace op_contabilidad_api.Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext (DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SubMayor>().HasKey(c => new { c.Analisis, c.Ano, c.Cta, c.Debe, c.Epigrafe, c.Fecha, c.Haber, c.Mes, c.SubAnalisis, c.SubCta });
            builder.Entity<ConceptoCuentas>().HasKey(c => new { c.ConceptoPlanId, c.CuentaId });
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<op_contabilidad_api.Models.Area> Area { get; set; }

        public DbSet<op_contabilidad_api.Models.PlanPronosticoProductivo> PlanPronosticoProductivo { get; set; }

        public DbSet<op_contabilidad_api.Models.CentroCostoArea> CentroCostoArea { get; set; }

        public DbSet<op_contabilidad_api.Models.GrupoSubelemento> GrupoSubelemento { get; set; }

        public DbSet<op_contabilidad_api.Models.GrupoSubElemento_SubElemento> GrupoSubElemento_SubElemento { get; set; }

        public DbSet<op_contabilidad_api.Models.Plan> Plan { get; set; }
        public DbSet<op_contabilidad_api.Models.SubMayor> SubMayor { get; set; }
        public DbSet<op_contabilidad_api.Models.PlanGI> PlanGI { get; set; }
        public DbSet<op_contabilidad_api.Models.SubMayorCuenta> SubMayorCuenta { get; set; }
    }
}
