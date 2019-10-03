using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.DbConfigurations;

namespace ContabilidadWebApi.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new AsientoConfig().Configure(modelBuilder.Entity<Asiento>());
            new CuentaConfig().Configure(modelBuilder.Entity<Cuenta>());
            new NivelConfig().Configure(modelBuilder.Entity<Nivel>());
            new MovimientoConfig().Configure(modelBuilder.Entity<Movimiento>());
            modelBuilder.ForNpgsqlUseIdentityColumns();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ContabilidadWebApi.Models.Area> Area { get; set; }

        public DbSet<ContabilidadWebApi.Models.PlanPronosticoProductivo> PlanPronosticoProductivo { get; set; }

        public DbSet<ContabilidadWebApi.Models.CentroCostoArea> CentroCostoArea { get; set; }

        public DbSet<ContabilidadWebApi.Models.GrupoSubelemento> GrupoSubelemento { get; set; }

        public DbSet<ContabilidadWebApi.Models.GrupoSubElemento_SubElemento> GrupoSubElemento_SubElemento { get; set; }

        public DbSet<ContabilidadWebApi.Models.Plan> Plan { get; set; }
        public DbSet<ContabilidadWebApi.Models.SubMayor> SubMayor { get; set; }
        public DbSet<ContabilidadWebApi.Models.PlanGI> PlanGI { get; set; }
        public DbSet<ContabilidadWebApi.Models.SubMayorCuenta> SubMayorCuenta { get; set; }
    }
}
