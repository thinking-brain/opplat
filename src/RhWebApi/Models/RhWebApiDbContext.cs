using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhWebApi.Models
{
     public class RhWebApiContext : DbContext
    {
        public RhWebApiContext(DbContextOptions<RhWebApiContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

       public DbSet<RhWebApi.Models.Trabajador> Trabajador { get; set; }
       public DbSet<RhWebApi.Models.Area> Area { get; set; }
       public DbSet<RhWebApi.Models.ActividadContrato> ActividadContrato { get; set; }
       public DbSet<RhWebApi.Models.HistoricoPuestoDeTrabajo> HistoricoPuestoDeTrabajo { get; set; }
       public DbSet<RhWebApi.Models.Cargo> Cargo { get; set; }
    //    public DbSet<RhWebApi.Models.GrupoEscala> GrupoEscala { get; set; }
    }
}
