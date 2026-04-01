using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Opplat.Infrastructure.Persistance.Data;

public class DesignTimeSalesDbContextFactory : IDesignTimeDbContextFactory<SalesDbContext>
{
    public SalesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SalesDbContext>();
        optionsBuilder.UseNpgsql(
            PostgresTenantConnectionStringResolver.ResolveDesignTime(null, "opplat_design"));
        return new SalesDbContext(optionsBuilder.Options, null);
    }
}
