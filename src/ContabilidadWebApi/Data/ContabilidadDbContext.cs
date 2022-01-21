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
            modelBuilder.Entity<ConceptoCuentas>().HasKey(c => new { c.ConceptoPlanId, c.CuentaId });
            new CuentaConfig().Configure(modelBuilder.Entity<Cuenta>());
            //new MovimientoConfig().Configure(modelBuilder.Entity<Movimiento>());
            modelBuilder.ForNpgsqlUseIdentityColumns();
            base.OnModelCreating(modelBuilder);



        }

        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<PlanGI> PlanesIngresosGastos { get; set; }
        //gastos
        public DbSet<ElementoDeGasto> ElementoDeGastos { get; set; }
        public DbSet<SubElementoDeGasto> SubElementoDeGastos { get; set; }
        public DbSet<CuentaElementoDeGasto> CuentaElementoDeGastos { get; set; }
        public DbSet<RegistroDeGasto> RegistroDeGastos { get; set; }
        public DbSet<PartidaDeGasto> PartidaDeGastos { get; set; }
        public DbSet<CentroDeCosto> CentroDeCostos { get; set; }
        //fin gastos

        // public DbSet<ContabilidadWebApi.Models.Area> Area { get; set; }

        // public DbSet<ContabilidadWebApi.Models.PlanPronosticoProductivo> PlanPronosticoProductivo { get; set; }

        //public DbSet<ContabilidadWebApi.Models.CentroCostoArea> CentroCostoArea { get; set; }

        //public DbSet<ContabilidadWebApi.Models.PlanGI> PlanesIngresosGastos { get; set; }

    }
}
