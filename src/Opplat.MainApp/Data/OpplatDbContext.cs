// using LicenceChecker;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Models;
using SalesEntities = Opplat.Domain.Sales.Entities;
using InventoryEntities = Opplat.Domain.Inventory.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Opplat.MainApp.Data;

public class OpplatDbContext : IdentityDbContext<Usuario>
{
    public OpplatDbContext(DbContextOptions options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseIdentityColumns();
        base.OnModelCreating(builder);
        builder.Entity<UserNotification>().HasKey(s => new { s.NotificationId, s.UsuarioId });
        builder.Entity<SalesEntities.AddedTopping>().HasKey(s => new { s.ToppingId, s.SaleDetailId });
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
        // Inventory
        builder.Entity<InventoryEntities.ProductInventory>().HasKey(s => new { s.ProductId, s.StorageId });
        // Sales
        builder.Entity<SalesEntities.CostTabDetail>().HasKey(s => new { s.ProductForSaleId, s.ProductId });
    }

    public DbSet<Licencia> Licencias { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }

    // Sales Entities
    public DbSet<SalesEntities.ProductForSale> ProductsForSale { get; set; }
    public DbSet<SalesEntities.Topping> Toppings { get; set; }
    public DbSet<SalesEntities.CostTab> CostTabs { get; set; }
    public DbSet<SalesEntities.ProductTag> ProductTags { get; set; }
    public DbSet<SalesEntities.Annotation> Annotations { get; set; }
    public DbSet<SalesEntities.Sale> Sales { get; set; }

    // Inventory Entities
    public DbSet<InventoryEntities.Product> Products { get; set; }
    public DbSet<InventoryEntities.ProductGroup> ProductGroups { get; set; }
    public DbSet<InventoryEntities.ProductClassification> ProductClassifications { get; set; }
    public DbSet<InventoryEntities.Storage> Storages { get; set; }
    public DbSet<InventoryEntities.ProductMovement> ProductMovements { get; set; }
    public DbSet<InventoryEntities.ProductInventory> Inventories { get; set; }
}
