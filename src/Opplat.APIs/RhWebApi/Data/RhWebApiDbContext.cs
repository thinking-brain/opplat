using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Models;

namespace RhWebApi.Data {
    public class RhWebApiDbContext : DbContext {
        public RhWebApiDbContext (DbContextOptions<RhWebApiDbContext> options) : base (options) { }
        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating (modelBuilder);
            modelBuilder.Entity<Provincia> ().HasData (
                new Provincia {
                    Id = 1,
                        Nombre = "Sin Definir"
                });
            modelBuilder.Entity<Municipio> ().HasData (
                new Municipio {
                    Id = 1,
                        Nombre = "Sin Definir",
                        ProvinciaId = 1
                });
            modelBuilder.Entity<PerfilOcupacional> ().HasData (
                new PerfilOcupacional {
                    Id = 1,
                        Nombre = "Sin Definir"
                });
            modelBuilder.Entity<CategoriaOcupacional> ().HasData (
                new CategoriaOcupacional {
                    Id = 1,
                        Descripcion = "Sin Definir"
                });
            modelBuilder.Entity<GrupoEscala> ().HasData (
                new GrupoEscala {
                    Id = 1,
                        CategoriaOcupacionalId = 1,
                        Codigo = "Sin Definir",
                        SalarioDiferenciado = false,
                        SalarioEscala = 0
                });
            modelBuilder.Entity<Cargo> ().HasData (
                new Cargo {
                    Id = 1,
                        Nombre = "Sin Definir",
                        Sigla = "Sin Definir",
                        GrupoEscalaId = 1
                });
            modelBuilder.Entity<PuestoDeTrabajo> ().HasData (
                new PuestoDeTrabajo {
                    Id = 1,
                        CargoId = 1,
                        UnidadOrganizativaId = 1,
                        Descripcion = "Sin Definir",
                        PlantillaOcupada = 0
                });
            modelBuilder.Entity<TipoUnidadOrganizativa> ().HasData (
                new TipoUnidadOrganizativa {
                    Id = 1,
                        Nombre = "Sin Definir",
                        Prioridad = 0
                });
            modelBuilder.Entity<UnidadOrganizativa> ().HasData (
                new UnidadOrganizativa {
                    Id = 1,
                        Codigo = "Sin Definir",
                        Nombre = "Sin Definir",
                        TipoUnidadOrganizativaId = 1,
                        Activa = false
                });
        }
        public DbSet<RhWebApi.Models.Trabajador> Trabajador { get; set; }
        public DbSet<RhWebApi.Models.Municipio> Municipio { get; set; }
        public DbSet<RhWebApi.Models.Cargo> Cargo { get; set; }
        public DbSet<RhWebApi.Models.ActividadLaboral> ActividadLaboral { get; set; }
        public DbSet<RhWebApi.Models.CaracteristicasTrab> CaracteristicasTrab { get; set; }
        public DbSet<RhWebApi.Models.CategoriaOcupacional> CategoriaOcupacional { get; set; }
        public DbSet<RhWebApi.Models.ContratoTrab> ContratoTrab { get; set; }
        public DbSet<RhWebApi.Models.ActividadContratoTrab> ActividadContratoTrab { get; set; }
        public DbSet<RhWebApi.Models.HistoricoPuestoDeTrabajo> HistoricoPuestoDeTrabajo { get; set; }
        public DbSet<RhWebApi.Models.PuestoDeTrabajo> PuestoDeTrabajo { get; set; }
        public DbSet<RhWebApi.Models.GrupoEscala> GrupoEscala { get; set; }
        public DbSet<RhWebApi.Models.Entrada> Entrada { get; set; }
        public DbSet<RhWebApi.Models.Baja> Baja { get; set; }
        public DbSet<RhWebApi.Models.Traslado> Traslado { get; set; }
        public DbSet<RhWebApi.Models.OtroMovimiento> OtroMovimiento { get; set; }
        public DbSet<RhWebApi.Models.UnidadOrganizativa> UnidadOrganizativa { get; set; }
        public DbSet<RhWebApi.Models.Plantilla> Plantilla { get; set; }
        public DbSet<RhWebApi.Models.Requisitos> Requisitos { get; set; }
        public DbSet<RhWebApi.Models.Funciones> Funciones { get; set; }
        public DbSet<RhWebApi.Models.Bolsa> Bolsa { get; set; }
        public DbSet<RhWebApi.Models.AperturaSocio> AperturaSocio { get; set; }
        public DbSet<RhWebApi.Models.PerfilOcupacional> PerfilOcupacional { get; set; }
        public DbSet<RhWebApi.Models.CaracteristicasSocio> CaracteristicasSocio { get; set; }
    }
}