using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Data {
<<<<<<< HEAD
    public class ContratacionDbContext : DbContext {
        public ContratacionDbContext (DbContextOptions<ContratacionDbContext> options) : base (options) {

        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            modelBuilder.ForNpgsqlUseIdentityColumns ();
            base.OnModelCreating (modelBuilder);
        }

        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Entidad> Entidades { get; set; }
        public DbSet<EspecialistaExterno> EspecialistasExternos { get; set; }
        public DbSet<FormaDePago> FormasDePago { get; set; }
        public DbSet<HistoricoEstadoContrato> HistEstDeContratos { get; set; }
=======
    public class ContratacionDbContext  : DbContext {
        public ContratacionDbContext  (DbContextOptions<ContratacionDbContext> options) : base (options) { }
        protected override void OnModelCreating (ModelBuilder builder) {
            base.OnModelCreating (builder);
        }
         public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Entidad> Entidades { get; set; }
        public DbSet<EspecialistaExterno> EspecialistasExternos { get; set; }
        public DbSet<FormaDePago> FormasDePagos { get; set; }
        public DbSet<HistoricoEstadoContrato> HistoricosEstadoContratos { get; set; }
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
    }
}