using Microsoft.EntityFrameworkCore;
using Opplat.AdminApi.Models;

namespace Opplat.AdminApi.Data;

public sealed class AdminTenantCatalogDbContext : DbContext
{
    public DbSet<AdminTenantInfo> Tenants => Set<AdminTenantInfo>();

    public AdminTenantCatalogDbContext(DbContextOptions<AdminTenantCatalogDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AdminTenantInfo>(tenant =>
        {
            tenant.ToTable("AdminTenants");
            tenant.HasIndex(model => model.Identifier).IsUnique();
            tenant.Property(model => model.Id).HasMaxLength(128);
            tenant.Property(model => model.Identifier).HasMaxLength(128);
            tenant.Property(model => model.Name).HasMaxLength(256);
            tenant.Property(model => model.DatabaseName).HasMaxLength(256);
            tenant.Property(model => model.DatabaseSchema).HasMaxLength(128);
        });
    }
}
