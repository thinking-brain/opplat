// using LicenceChecker;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Options;
using Opplat.Domain.Sales.Entities;


namespace Opplat.MainApp.Data;

public class OpplatDbContext : ApiAuthorizationDbContext<Usuario>
{
    public OpplatDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
        : base(options, operationalStoreOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseIdentityColumns();
        base.OnModelCreating(builder);
        builder.Entity<UserNotification>().HasKey(s => new { s.NotificationId, s.UsuarioId });
        builder.Entity<AddedTopping>().HasKey(s => new { s.ToppingId, s.SaleDetailId });
        builder.Entity<IdentityRole>().HasData(new IdentityRole[] {
                    new IdentityRole {Id = "1", Name = "administrador", NormalizedName = "ADMINISTRADOR" },
            });
        builder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = "f42559a2-2776-4e9b-9ba1-268597eff72b",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@opplat.cu",
                NormalizedEmail = "ADMIN@OPPLAT.CU",
                PasswordHash = "AQAAAAEAACcQAAAAEP4OedI6m26WUn/2C4AcBkzdT6SnL/6E+xakQ/9mGAkqqp3t9PwyIR6l9obLouKIVg==",
                SecurityStamp = "43VMKYQKNTENYZVJNU2TII26X23H5PGV",
                ConcurrencyStamp = "36fd2616-8e8a-4cc6-8a5a-52d963207836",
                Activo = true,
                Nombres = "Administrador",
                Apellidos = "General",
            }
            );
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = "f42559a2-2776-4e9b-9ba1-268597eff72b",
                RoleId = "1"
            }
            );
    }

    public DbSet<Licencia> Licencias { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<UserNotification> UserNotifications { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Topping> Toppings { get; set; } = null!;
    public DbSet<Annotation> Annotations { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
}
