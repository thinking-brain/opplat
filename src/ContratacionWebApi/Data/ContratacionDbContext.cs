using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Data {
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
        public DbSet<FormaDePago> FormasDePagos { get; set; }
        public DbSet<HistoricoEstadoContrato> HistoricosEstadoContratos { get; set; }
    }
}