using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services;

namespace Opplat.Infrastructure.DependencyInjection;

public static class AdminDatabaseExtensions
{
    public static IServiceCollection AddAdminDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Singleton factory — safe to inject into singleton services like TenantCatalogStore.
        services.AddDbContextFactory<AdminTenantCatalogDbContext>(
            options => options.UseNpgsql(NormalizePostgresConnectionString(
                configuration.GetConnectionString("AdminDatabase"))));

        // Scoped context — satisfies services that inject AdminTenantCatalogDbContext directly.
        services.AddScoped<AdminTenantCatalogDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<AdminTenantCatalogDbContext>>()
              .CreateDbContext());

        // Module 3: Provisioning & Migration Services
        services.Configure<TenantDatabaseOptions>(configuration.GetSection(TenantDatabaseOptions.SectionName));
        var tenantDatabaseOptions = configuration.GetSection(TenantDatabaseOptions.SectionName)
            .Get<TenantDatabaseOptions>() ?? new TenantDatabaseOptions();
        services.AddSingleton(tenantDatabaseOptions);

        services.Configure<DatabaseInstanceOptions>(configuration.GetSection(DatabaseInstanceOptions.SectionName));
        var databaseInstanceOptions = configuration.GetSection(DatabaseInstanceOptions.SectionName)
            .Get<DatabaseInstanceOptions>() ?? new DatabaseInstanceOptions();
        services.AddSingleton(databaseInstanceOptions);
        services.AddScoped<TenantSchemaProvisioningService>();
        services.AddScoped<DatabaseInstanceAutoScalingService>();
        services.AddScoped<ITenantSchemaMigrationRunner, TenantSchemaMigrationRunner>();
        services.AddScoped<ITenantProvisioningCoordinator, TenantProvisioningCoordinator>();
        services.AddSingleton<IEnumerable<Infrastructure.Services.ITenantProvisioningReporter>>([]);
        services.AddSingleton<IEnumerable<ITenantSchemaMigrationReporter>>([]);

        return services;
    }

    private static string NormalizePostgresConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:AdminDatabase must be configured.");

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;

        return builder.ConnectionString;
    }
}
