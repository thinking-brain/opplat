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
                configuration.GetConnectionString("DefaultConnection"))));

        // Module 3: Provisioning & Migration Services
        var databaseInstanceOptions = configuration.GetSection(DatabaseInstanceOptions.SectionName)
            .Get<DatabaseInstanceOptions>() ?? new DatabaseInstanceOptions();
        
        if (string.IsNullOrWhiteSpace(databaseInstanceOptions.DefaultConnectionString))
        {
            databaseInstanceOptions.DefaultConnectionString =
                "Host=localhost;Port=5432;Database=opplat_tenants_db1;Username=postgres;Password=Admin123*";
        }

        services.Configure<DatabaseInstanceOptions>(configuration.GetSection(DatabaseInstanceOptions.SectionName));
        services.AddSingleton(databaseInstanceOptions);
        services.AddScoped<TenantSchemaProvisioningService>();
        services.AddScoped<DatabaseInstanceAutoScalingService>();
        services.AddScoped<ITenantSchemaMigrationRunner, TenantSchemaMigrationRunner>();
        services.AddScoped<ITenantProvisioningCoordinator, TenantProvisioningCoordinator>();
        services.AddSingleton<IEnumerable<Opplat.Infrastructure.Services.ITenantProvisioningReporter>>([]);
        services.AddSingleton<IEnumerable<Opplat.Application.Abstractions.Services.ITenantSchemaMigrationReporter>>([]);

        return services;
    }

    private static string NormalizePostgresConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;

        return builder.ConnectionString;
    }
}
