using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Opplat.Microservices.Shared.Models;
using SalesEntities = Opplat.Modules.Sales.Domain.Entities;

namespace Opplat.Services.Sales.Api.Data;

public class SalesDbContext : DbContext, IMultiTenantDbContext
{
    private readonly IMultiTenantContextAccessor<AppTenantInfo>? _tenantAccessor;

    public SalesDbContext(
        DbContextOptions<SalesDbContext> options,
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
        builder.Entity<SalesEntities.AddedTopping>().HasKey(entity => new { entity.ToppingId, entity.SaleDetailId });
        builder.Entity<SalesEntities.CostTabDetail>().HasKey(entity => new { entity.ProductForSaleId, entity.ProductId });
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

    public DbSet<SalesEntities.ProductForSale> ProductsForSale => Set<SalesEntities.ProductForSale>();

    public DbSet<SalesEntities.Topping> Toppings => Set<SalesEntities.Topping>();

    public DbSet<SalesEntities.CostTab> CostTabs => Set<SalesEntities.CostTab>();

    public DbSet<SalesEntities.ProductTag> ProductTags => Set<SalesEntities.ProductTag>();

    public DbSet<SalesEntities.Annotation> Annotations => Set<SalesEntities.Annotation>();

    public DbSet<SalesEntities.Sale> Sales => Set<SalesEntities.Sale>();
}
