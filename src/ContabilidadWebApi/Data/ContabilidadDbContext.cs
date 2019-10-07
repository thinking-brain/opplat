using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.DbConfigurations;

namespace ContabilidadWebApi.Data
{
    public class ContabilidadDbContext : DbContext
    {
        public ContabilidadDbContext(DbContextOptions<ContabilidadDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //new AsientoConfig().Configure(modelBuilder.Entity<Asiento>());
            new CuentaConfig().Configure(modelBuilder.Entity<Cuenta>());
            //new MovimientoConfig().Configure(modelBuilder.Entity<Movimiento>());
            modelBuilder.ForNpgsqlUseIdentityColumns();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Cuenta> Cuentas { get; set; }

        // public DbSet<ContabilidadWebApi.Models.Area> Area { get; set; }

        // public DbSet<ContabilidadWebApi.Models.PlanPronosticoProductivo> PlanPronosticoProductivo { get; set; }

        //public DbSet<ContabilidadWebApi.Models.CentroCostoArea> CentroCostoArea { get; set; }

        //public DbSet<ContabilidadWebApi.Models.PlanGI> PlanesIngresosGastos { get; set; }

    }
}
