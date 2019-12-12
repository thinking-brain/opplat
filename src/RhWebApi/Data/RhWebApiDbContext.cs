using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Models;

namespace RhWebApi.Data 
{
    public class RhWebApiDbContext : DbContext {
        public RhWebApiDbContext (DbContextOptions<RhWebApiDbContext> options) : base (options) { }
        protected override void OnModelCreating (ModelBuilder builder) {
            base.OnModelCreating (builder);
            // Customize the ASP.NET Identity model   override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<RhWebApi.Models.Trabajador> Trabajador { get; set; }
        public DbSet<RhWebApi.Models.Municipio> Municipio { get; set; }
        public DbSet<RhWebApi.Models.Cargo> Cargo { get; set; }
        public DbSet<RhWebApi.Models.ActividadLaboral> ActividadLaboral { get; set; }       
        public DbSet<RhWebApi.Models.CaracteristicasTrab> CaracteristicasTrab { get; set; }
        public DbSet<RhWebApi.Models.CategoriaOcupacional> CategoriaOcupacional { get; set; }
        public DbSet<RhWebApi.Models.Contrato> Contrato { get; set; }
        public DbSet<RhWebApi.Models.ActividadContrato> ActividadContrato { get; set; }
        public DbSet<RhWebApi.Models.HistoricoPuestoDeTrabajo> HistoricoPuestoDeTrabajo { get; set; }
        public DbSet<RhWebApi.Models.PuestoDeTrabajo> PuestoDeTrabajo { get; set; }
        public DbSet<RhWebApi.Models.GrupoEscala> GrupoEscala { get; set; }
         public DbSet<RhWebApi.Models.Entrada> Entrada { get; set; }
        public DbSet<RhWebApi.Models.Baja> Baja { get; set; }
        public DbSet<RhWebApi.Models.Traslado> Traslado { get; set; }
        public DbSet<RhWebApi.Models.OtroMovimiento> OtroMovimiento { get; set; }
        public DbSet<RhWebApi.Models.UnidadOrganizativa> UnidadOrganizativa { get; set; }
        public DbSet<RhWebApi.Models.Plantilla> Plantilla { get; set; }
    }
}