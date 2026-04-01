using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Opplat.Infrastructure.Persistance.Data;

public class DesignTimeInventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseNpgsql(
            PostgresTenantConnectionStringResolver.ResolveDesignTime(null, "opplat_design"));
        return new InventoryDbContext(optionsBuilder.Options, null);
    }
}
