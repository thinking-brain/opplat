using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzasWebApi.Data
{
    public class FinanzasDbContext : DbContext
    {
        public FinanzasDbContext(DbContextOptions<FinanzasDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ForNpgsqlUseIdentityColumns();
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<ConfiguracionFirmas> ConfiguracionesFirmas { get; set; }
        public DbSet<ConfiguracionPorciento> ConfiguracionesPorcientos { get; set; }
        public DbSet<CacheCuentaPeriodo> CachesCuentasEnPeriodos { get; set; }
        public DbSet<CacheEstadoFinanciero> CachesEstadosFinancieros { get; set; }


    }
}
