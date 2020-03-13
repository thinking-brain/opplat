using ContratacionWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Data
{
    public class ContratacionDbContext : DbContext {
        public ContratacionDbContext (DbContextOptions<ContratacionDbContext> options) : base (options) {

        }
        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            modelBuilder.ForNpgsqlUseIdentityColumns ();
            base.OnModelCreating (modelBuilder);
        }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<ContratoId_DictaminadorId> ContratoId_DictaminadorId { get; set; }
        public DbSet<DictaminadorContrato> DictaminadoresContrato { get; set; }
        public DbSet<Entidad> Entidades { get; set; }
        public DbSet<EspecialistaExterno> EspecialistasExternos { get; set; }
        public DbSet<EspExternoId_ContratoId> EspExternoId_ContratoId { get; set; }
        public DbSet<FormaDePago> FormasDePagos { get; set; }
        public DbSet<HistoricoEstadoContrato> HistoricosEstadoContratos { get; set; }
    }
}