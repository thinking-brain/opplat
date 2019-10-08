using Microsoft.EntityFrameworkCore;
using InventarioWebApi.Models;

namespace InventarioWebApi.Data
{
    public class InventarioContext : DbContext
    {
        public InventarioContext(DbContextOptions<InventarioContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Submayor>().HasKey(k => new { k.AlmacenId, k.ProductoId });
        }

        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Submayor> Submayores { get; set; }
        public DbSet<Derivado> Derivados { get; set; }
    }

}