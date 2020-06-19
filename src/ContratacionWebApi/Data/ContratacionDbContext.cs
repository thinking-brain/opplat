using ContratacionWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Data {
    public class ContratacionDbContext : DbContext {
        public ContratacionDbContext (DbContextOptions<ContratacionDbContext> options) : base (options) {

        }
        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            modelBuilder.Entity<EspExternoId_ContratoId> ()
                .HasKey (x => new { x.ContratoId, x.EspecialistaExternoId });
            modelBuilder.ForNpgsqlUseIdentityColumns ();
            base.OnModelCreating (modelBuilder);
        }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<ContratoId_DictaminadorId> ContratoId_DictaminadorId { get; set; }
        public DbSet<DictaminadorContrato> DictaminadoresContratos { get; set; }
        public DbSet<Entidad> Entidades { get; set; }
        public DbSet<CuentaBancaria> CuentasBancarias { get; set; }
        public DbSet<EspecialistaExterno> EspecialistasExternos { get; set; }
        public DbSet<EspExternoId_ContratoId> EspExternoId_ContratoId { get; set; }
        public DbSet<ContratoId_FormaPagoId> ContratoId_FormaPagoId { get; set; }
        public DbSet<HistoricoEstadoContrato> HistoricosEstadoContratos { get; set; }
        public DbSet<AdminContrato> AdminContratos { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Telefono> Telefonos { get; set; }
        public DbSet<TiempoVenOferta> TiempoVenOfertas { get; set; }
        public DbSet<TiempoVenContrato> TiempoVenContratos { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<ContratoId_DepartamentoId> ContratoId_DepartamentoId { get; set; }
    }
}