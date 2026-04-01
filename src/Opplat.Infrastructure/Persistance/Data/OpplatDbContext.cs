using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesEntities = Opplat.Domain.Entities.Sales;
using InventoryEntities = Opplat.Domain.Entities.Inventory;
using Opplat.Domain.Models;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;

namespace Opplat.Infrastructure.Persistance.Data;

public class OpplatDbContext(
    DbContextOptions<OpplatDbContext> options,
    IMultiTenantContextAccessor<AppTenantInfo>? tenantAccessor = null) : DbContext(options), IMultiTenantDbContext
{
    private readonly IMultiTenantContextAccessor<AppTenantInfo>? _tenantAccessor = tenantAccessor;
    
    public ITenantInfo? TenantInfo => _tenantAccessor?.MultiTenantContext?.TenantInfo;
    public TenantMismatchMode TenantMismatchMode => TenantMismatchMode.Throw;
    public TenantNotSetMode TenantNotSetMode => TenantNotSetMode.Throw;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseIdentityByDefaultColumns();
        base.OnModelCreating(builder);
        
        builder.ConfigureMultiTenant();
        
        builder.ApplyConfigurationsFromAssembly(
            typeof(OpplatDbContext).Assembly,
            t => t.Namespace != null && (
                t.Namespace.StartsWith("Opplat.Infrastructure.Persistance.Configurations.Sales") ||
                t.Namespace.StartsWith("Opplat.Infrastructure.Persistance.Configurations.Inventory") ||
                t.Namespace.StartsWith("Opplat.Infrastructure.Persistance.Configurations.Core")));
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

    public DbSet<License> Licenses { get; set; }
    public DbSet<User> Users { get; set; }
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
