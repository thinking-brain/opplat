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
            modelBuilder.Entity<MovimientoDeProducto>().Property(m => m.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<TipoMovimiento>().HasData(
                new TipoMovimiento { Id = 1, Descripcion = "Entrada", Factor = 1 },
                new TipoMovimiento { Id = 2, Descripcion = "Entrada traslado interno", Factor = 1 },
                new TipoMovimiento { Id = 3, Descripcion = "Merma", Factor = -1 },
                new TipoMovimiento { Id = 4, Descripcion = "Salida a producción", Factor = -1 },
                new TipoMovimiento { Id = 5, Descripcion = "Salida traslado interno", Factor = -1 },
                new TipoMovimiento { Id = 6, Descripcion = "Venta independiente", Factor = -1 },
                new TipoMovimiento { Id = 7, Descripcion = "Entrada por error en salida", Factor = 1 },
                new TipoMovimiento { Id = 8, Descripcion = "Salida por error en entrada", Factor = -1 },
                new TipoMovimiento { Id = 9, Descripcion = "Entrada por ajuste", Factor = 1 },
                new TipoMovimiento { Id = 10, Descripcion = "Salida", Factor = -1 },
                new TipoMovimiento { Id = 11, Descripcion = "Entrada por conversión de producto", Factor = 1 },
                new TipoMovimiento { Id = 12, Descripcion = "Salida por conversion de producto", Factor = -1 }
            );
            modelBuilder.Entity<TipoUnidadDeMedida>().HasData(
                new TipoUnidadDeMedida { Id = 1, Nombre = "Masa" }
            );
            modelBuilder.Entity<UnidadDeMedida>().HasData(
                new UnidadDeMedida { Id = 1, Descripcion = "g", Nombre = "Gramo", FactorConversion = 1, TipoId = 1 },
                new UnidadDeMedida { Id = 2, Descripcion = "kg", Nombre = "Kilogramo", FactorConversion = 1000, TipoId = 1 },
                new UnidadDeMedida { Id = 3, Descripcion = "q", Nombre = "Quintal", FactorConversion = 100000, TipoId = 1 },
                new UnidadDeMedida { Id = 4, Descripcion = "t", Nombre = "Tonelada", FactorConversion = 1000000, TipoId = 1 },
                new UnidadDeMedida { Id = 5, Descripcion = "lb", Nombre = "Libra", FactorConversion = 453.59M, TipoId = 1 }
            );
        }

        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Submayor> Submayores { get; set; }
        public DbSet<Derivado> Derivados { get; set; }
        public DbSet<MovimientoDeProducto> MovimientosDeProductos { get; set; }
        public DbSet<UnidadDeMedida> UnidadesDeMedida { get; set; }
        public DbSet<TipoMovimiento> TiposDeMovimiento { get; set; }
        public DbSet<TipoUnidadDeMedida> TiposUnidadDeMedida { get; set; }
    }

}