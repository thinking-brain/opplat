using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TallerWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TallerWebApi.Data {
    public class TallerWebApiDbContext : DbContext {
        public TallerWebApiDbContext (DbContextOptions<TallerWebApiDbContext> options) : base (options) { }
        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating (modelBuilder);
        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DocumentoEquipo> DocumentosEquipos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<HistoricoEquipo> HistoricoEquipos { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<OrdenReparacion_Repuesto> OrdenesReparaciones_Repuesto { get; set; }
        public DbSet<OrdenReparacion> OrdenesReparacion { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<RMA> RMAs { get; set; }
        public DbSet<Taller> Talleres { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }
        public DbSet<TipoEquipo> TiposEquipos { get; set; }
    }
}