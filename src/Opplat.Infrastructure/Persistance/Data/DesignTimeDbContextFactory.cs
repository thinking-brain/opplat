using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Opplat.Infrastructure.Persistance.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OpplatDbContext>
{
    public OpplatDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("appsettings.json", optional: true)
            // .AddJsonFile("appsettings.Development.json", optional: true)
            // .AddEnvironmentVariables()
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<OpplatDbContext>();
        optionsBuilder.UseNpgsql(
            PostgresTenantConnectionStringResolver.ResolveDesignTime(
                configuration.GetConnectionString("DefaultConnection")
                    ?? configuration.GetConnectionString("MainConnection"),
                "opplat_design"));
        return new OpplatDbContext(optionsBuilder.Options, null);
    }
}
