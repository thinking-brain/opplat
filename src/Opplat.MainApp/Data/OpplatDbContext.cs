// using LicenceChecker;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Models;
using SalesEntities = Opplat.Modules.Sales.Domain.Entities;
using InventoryEntities = Opplat.Modules.Inventory.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Opplat.MainApp.Data;

public class OpplatDbContext : IdentityDbContext<Usuario>, IMultiTenantDbContext
{
    private readonly IMultiTenantContextAccessor<AppTenantInfo>? _tenantAccessor;
    
    public ITenantInfo? TenantInfo => _tenantAccessor?.MultiTenantContext?.TenantInfo;
    public TenantMismatchMode TenantMismatchMode => TenantMismatchMode.Throw;
    public TenantNotSetMode TenantNotSetMode => TenantNotSetMode.Throw;
    
    public OpplatDbContext(
        DbContextOptions<OpplatDbContext> options,
        IMultiTenantContextAccessor<AppTenantInfo>? tenantAccessor = null)
        : base(options)
    {
        _tenantAccessor = tenantAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseIdentityByDefaultColumns();
        base.OnModelCreating(builder);
        
        builder.ConfigureMultiTenant();
        
        builder.Entity<UserNotification>().HasKey(s => new { s.NotificationId, s.UsuarioId });
        builder.Entity<SalesEntities.AddedTopping>().HasKey(s => new { s.ToppingId, s.SaleDetailId });
        builder.Entity<InventoryEntities.ProductInventory>().HasKey(s => new { s.ProductId, s.StorageId });
        builder.Entity<SalesEntities.CostTabDetail>().HasKey(s => new { s.ProductForSaleId, s.ProductId });
    }
    
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.EnforceMultiTenant();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.EnforceMultiTenant();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
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
