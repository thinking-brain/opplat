using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Opplat.Domain.Models;
using InventoryEntities = Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Data;

public class InventoryDbContext : DbContext, IMultiTenantDbContext
{
    private readonly IMultiTenantContextAccessor<AppTenantInfo>? _tenantAccessor;

    public InventoryDbContext(
        DbContextOptions<InventoryDbContext> options,
        IMultiTenantContextAccessor<AppTenantInfo>? tenantAccessor = null)
        : base(options)
    {
        _tenantAccessor = tenantAccessor;
    }

    public ITenantInfo? TenantInfo => _tenantAccessor?.MultiTenantContext?.TenantInfo;

    public TenantMismatchMode TenantMismatchMode => TenantMismatchMode.Throw;

    public TenantNotSetMode TenantNotSetMode => TenantNotSetMode.Throw;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureMultiTenant();
        builder.ApplyConfigurationsFromAssembly(
            typeof(InventoryDbContext).Assembly,
            t => t.Namespace != null && (
                t.Namespace.StartsWith("Opplat.Infrastructure.Persistance.Configurations.Common") ||
                t.Namespace.StartsWith("Opplat.Infrastructure.Persistance.Configurations.Inventory")));
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.EnforceMultiTenant();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        this.EnforceMultiTenant();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public DbSet<InventoryEntities.Product> Products => Set<InventoryEntities.Product>();

    public DbSet<InventoryEntities.ProductGroup> ProductGroups => Set<InventoryEntities.ProductGroup>();

    public DbSet<InventoryEntities.ProductClassification> ProductClassifications => Set<InventoryEntities.ProductClassification>();

    public DbSet<InventoryEntities.Storage> Storages => Set<InventoryEntities.Storage>();

    public DbSet<InventoryEntities.ProductMovement> ProductMovements => Set<InventoryEntities.ProductMovement>();

    public DbSet<InventoryEntities.ProductInventory> Inventories => Set<InventoryEntities.ProductInventory>();
}
