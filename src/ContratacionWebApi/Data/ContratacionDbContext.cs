using ContratacionWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Data
{
    public class ContratacionDbContext : DbContext
    {
        public ContratacionDbContext(DbContextOptions<ContratacionDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlUseIdentityColumns();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<FormaDePago> FormasDePago { get; set; }
        public DbSet<HistoricoEstadoContrato> HistoricosDeEstado { get; set; }
        public DbSet<Entidad> Entidades { get; set; }
    }
}