using Microsoft.EntityFrameworkCore;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Infrastructure.Persistance.Data.Administration;

public sealed class AdminTenantCatalogDbContext(DbContextOptions<AdminTenantCatalogDbContext> options) : DbContext(options)
{
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<DatabaseInstance> DatabaseInstances => Set<DatabaseInstance>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<TenantPaymentMethod> TenantPaymentMethods => Set<TenantPaymentMethod>();
    public DbSet<SubscriptionInvoice> SubscriptionInvoices => Set<SubscriptionInvoice>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AdminTenantCatalogDbContext).Assembly,
            t => t.Namespace != null && (
                t.Namespace.StartsWith("Opplat.Infrastructure.Persistance.Configurations.Common") ||
                t.Namespace.StartsWith("Opplat.Infrastructure.Persistance.Configurations.Administration")));
    }
}
