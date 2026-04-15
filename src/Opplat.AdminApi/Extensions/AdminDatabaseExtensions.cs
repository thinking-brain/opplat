using Microsoft.EntityFrameworkCore;
using Npgsql;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Infrastructure.Persistance.Data.Administration;
using Opplat.Infrastructure.Services;

namespace Opplat.AdminApi.Extensions;

public static class AdminDatabaseExtensions
{
    public static IServiceCollection AddAdminDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AdminTenantCatalogDbContext>(options =>
            options.UseNpgsql(NormalizePostgresConnectionString(
                configuration.GetConnectionString("AdminDatabase"))));

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
