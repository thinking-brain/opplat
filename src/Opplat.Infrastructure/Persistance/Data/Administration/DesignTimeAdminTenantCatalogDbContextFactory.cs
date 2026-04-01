using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Opplat.Infrastructure.Persistance.Data.Administration;

public class DesignTimeAdminTenantCatalogDbContextFactory : IDesignTimeDbContextFactory<AdminTenantCatalogDbContext>
{
    public AdminTenantCatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AdminTenantCatalogDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=opplat_admin_design;Username=postgres;Password=postgres");
        return new AdminTenantCatalogDbContext(optionsBuilder.Options);
    }
}
