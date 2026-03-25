using Npgsql;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Data;

internal static class PostgresTenantConnectionStringResolver
{
    public static string Resolve(AppTenantInfo? tenantInfo, string defaultConnectionString)
    {
        if (!string.IsNullOrWhiteSpace(tenantInfo?.ConnectionString))
            return Normalize(tenantInfo.ConnectionString!, tenantInfo.DatabaseSchema);

        if (string.IsNullOrWhiteSpace(tenantInfo?.DatabaseName))
            return Normalize(defaultConnectionString, tenantInfo?.DatabaseSchema);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(defaultConnectionString)
        {
            Database = tenantInfo.DatabaseName.Trim()
        };

        if (!string.IsNullOrWhiteSpace(tenantInfo.DatabaseSchema))
            connectionStringBuilder.SearchPath = tenantInfo.DatabaseSchema.Trim();

        return connectionStringBuilder.ConnectionString;
    }

    public static string ResolveDesignTime(string? connectionString, string fallbackDatabaseName) =>
        Resolve(
            new AppTenantInfo
            {
                DatabaseName = fallbackDatabaseName
            },
            string.IsNullOrWhiteSpace(connectionString)
                ? "Host=localhost;Port=5432;Username=postgres;Password=Admin123*;Database=postgres"
                : connectionString);

    private static string Normalize(string connectionString, string? databaseSchema)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        if (connectionStringBuilder.SslMode == SslMode.Prefer)
            connectionStringBuilder.SslMode = SslMode.Disable;

        if (!string.IsNullOrWhiteSpace(databaseSchema) &&
            string.IsNullOrWhiteSpace(connectionStringBuilder.SearchPath))
        {
            connectionStringBuilder.SearchPath = databaseSchema.Trim();
        }

        return connectionStringBuilder.ConnectionString;
    }
}
