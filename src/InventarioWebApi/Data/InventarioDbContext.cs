using Microsoft.EntityFrameworkCore;
using InventarioWebApi.Models;

namespace InventarioWebApi.Data
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Submayor>().HasKey(k => new { k.AlmacenId, k.ProductoId });
            modelBuilder.Entity<Almacen>().Property(k => k.Id).ValueGeneratedOnAdd();
        }

        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Submayor> Submayores { get; set; }
        public DbSet<Derivado> Derivados { get; set; }
        public DbSet<MovimientoDeProducto> MovimientosDeProductos { get; set; }
        public DbSet<UnidadDeMedida> UnidadesDeMedida { get; set; }
        public DbSet<TipoMovimiento> TiposDeMovimiento { get; set; }
    }

}