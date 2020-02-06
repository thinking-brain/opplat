using Microsoft.EntityFrameworkCore;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class EnlaceVersatDbContext : DbContext
    {
        public EnlaceVersatDbContext(DbContextOptions<EnlaceVersatDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlUseIdentityColumns();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Asientos> Asientos { get; set; }
        public DbSet<Cuentas> Cuentas { get; set; }
        public DbSet<PeriodosContables> PeriodosContables { get; set; }
        public DbSet<Trabajador> Trabajadores { get; set; }
        public DbSet<UnidadOrganizativa> UnidadesOrganizativas { get; set; }
        //gastos
        public DbSet<ElementoDeGasto> ElementoDeGastos { get; set; }
        public DbSet<SubElementoDeGasto> SubElementoDeGastos { get; set; }
        public DbSet<PartidaDeGasto> PartidaDeGastos { get; set; }
    }
}