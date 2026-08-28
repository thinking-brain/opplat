using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data;
using System.Collections.Concurrent;
using System.Data;

namespace Opplat.Infrastructure.Services;

public class TenantProvisioningService(IServiceProvider serviceProvider, ILogger<TenantProvisioningService> logger)
{
    private static readonly ConcurrentDictionary<string, byte> ProvisionedTenants = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> ProvisioningLocks = new(StringComparer.OrdinalIgnoreCase);
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<TenantProvisioningService> _logger = logger;

    internal static bool ShouldSkipProvisioning(
        ConcurrentDictionary<string, byte> provisionedTenants,
        string tenantKey,
        bool tenantSchemaReady)
        => provisionedTenants.ContainsKey(tenantKey) && tenantSchemaReady;

    internal static bool IsRecoverableMigrationError(PostgresException ex)
    {
        if (ex is null)
            return false;

        if (ex.SqlState is "42P07" or "42710")
            return true;

        var message = ex.Message ?? string.Empty;
        if (!message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            return false;

        return message.Contains("relation", StringComparison.OrdinalIgnoreCase)
            || message.Contains("table", StringComparison.OrdinalIgnoreCase)
            || message.Contains("index", StringComparison.OrdinalIgnoreCase)
            || message.Contains("sequence", StringComparison.OrdinalIgnoreCase)
            || message.Contains("constraint", StringComparison.OrdinalIgnoreCase)
            || message.Contains("type", StringComparison.OrdinalIgnoreCase)
            || message.Contains("schema", StringComparison.OrdinalIgnoreCase);
    }

    public async Task ProvisionTenantAsync(AppTenantInfo tenantInfo)
    {
        var tenantKey = BuildTenantKey(tenantInfo);
        var tenantLock = ProvisioningLocks.GetOrAdd(tenantKey, _ => new SemaphoreSlim(1, 1));
        await tenantLock.WaitAsync();

        try
        {
            var tenantSchemaReady = false;
            if (ProvisionedTenants.ContainsKey(tenantKey))
            {
                using var readinessScope = _serviceProvider.CreateScope();
                var readinessConfiguration = readinessScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var readinessTenantDbOptions = readinessConfiguration.GetSection(TenantDatabaseOptions.SectionName).Get<TenantDatabaseOptions>()
                    ?? new TenantDatabaseOptions();
                var readinessDefaultConnectionString = new NpgsqlConnectionStringBuilder
                {
                    Host = readinessTenantDbOptions.Host,
                    Port = readinessTenantDbOptions.Port,
                    Username = readinessTenantDbOptions.Username,
                    Password = readinessTenantDbOptions.Password,
                    Database = "postgres",
                    SslMode = SslMode.Disable
                }.ConnectionString;
                var readinessTenantConnectionString = PostgresTenantConnectionStringResolver.Resolve(
                    tenantInfo,
                    readinessDefaultConnectionString);
                tenantSchemaReady = await IsTenantSchemaReadyAsync(readinessTenantConnectionString, tenantInfo.DatabaseSchema);
            }

            if (ShouldSkipProvisioning(ProvisionedTenants, tenantKey, tenantSchemaReady))
                return;

            using var scope = _serviceProvider.CreateScope();
            var provisioningConfiguration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var provisioningTenantDbOptions = provisioningConfiguration.GetSection(TenantDatabaseOptions.SectionName).Get<TenantDatabaseOptions>()
                ?? new TenantDatabaseOptions();
            var provisioningDefaultConnectionString = new NpgsqlConnectionStringBuilder
            {
                Host = provisioningTenantDbOptions.Host,
                Port = provisioningTenantDbOptions.Port,
                Username = provisioningTenantDbOptions.Username,
                Password = provisioningTenantDbOptions.Password,
                Database = "postgres",
                SslMode = SslMode.Disable
            }.ConnectionString;
            var provisioningTenantConnectionString = PostgresTenantConnectionStringResolver.Resolve(
                tenantInfo,
                provisioningDefaultConnectionString);

            await EnsureDatabaseExistsAsync(provisioningTenantConnectionString);
            await EnsureSchemaExistsAsync(provisioningTenantConnectionString, tenantInfo.DatabaseSchema);

            await RunTenantMigrationsAsync(tenantInfo, provisioningTenantConnectionString);
            tenantSchemaReady = await IsTenantSchemaReadyAsync(provisioningTenantConnectionString, tenantInfo.DatabaseSchema);

            if (tenantSchemaReady)
            {
                ProvisionedTenants[tenantKey] = 0;
            }
            else
            {
                ProvisionedTenants.TryRemove(tenantKey, out _);
                _logger.LogWarning(
                    "Tenant {Tenant} schema {Schema} is not ready after provisioning; leaving it unmarked so provisioning will retry.",
                    tenantInfo.Identifier,
                    tenantInfo.DatabaseSchema);
            }
        }
        finally
        {
            tenantLock.Release();
        }
    }

    private async Task RunTenantMigrationsAsync(AppTenantInfo tenantInfo, string connectionString)
    {
        var schema = tenantInfo.DatabaseSchema;
        if (string.IsNullOrWhiteSpace(schema))
            return;

        var migrationConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            SearchPath = schema
        }.ConnectionString;

        _logger.LogInformation("Applying EF Core migrations for tenant {Tenant} in schema {Schema}", tenantInfo.Identifier, schema);

        await RunMigrationsForContextAsync<OpplatDbContext>(migrationConnectionString, tenantInfo, schema);

        _logger.LogInformation("Completed EF Core migrations for tenant {Tenant} in schema {Schema}", tenantInfo.Identifier, schema);
    }

    private async Task RunMigrationsForContextAsync<TDbContext>(string connectionString, AppTenantInfo tenantInfo, string schema)
        where TDbContext : DbContext
    {
        try
        {
            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            await using var db = (TDbContext)Activator.CreateInstance(typeof(TDbContext), options, null)!;
            var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToList();

            foreach (var pendingMigration in pendingMigrations)
            {
                try
                {
                    await db.Database.MigrateAsync(pendingMigration);
                }
                catch (Exception ex) when (ex is PostgresException postgresException && IsRecoverableMigrationError(postgresException))
                {
                    _logger.LogWarning(postgresException,
                        "Tenant {Tenant} hit a recoverable migration error while applying migration {Migration} in schema {Schema}; recording it as applied so provisioning can continue.",
                        tenantInfo.Identifier,
                        pendingMigration,
                        schema);

                    await MarkMigrationAsAppliedAsync(db, pendingMigration);
                }
            }
        }
        catch (Exception ex) when (ex is PostgresException postgresException && IsRecoverableMigrationError(postgresException))
        {
            _logger.LogWarning(postgresException,
                "Tenant {Tenant} already has some objects in schema {Schema}; continuing after recoverable migration error {SqlState}",
                tenantInfo.Identifier,
                schema,
                postgresException.SqlState);
        }
    }

    private static async Task MarkMigrationAsAppliedAsync<TDbContext>(TDbContext db, string migrationName)
        where TDbContext : DbContext
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = """
            SELECT 1
            FROM "__EFMigrationsHistory"
            WHERE "MigrationId" = @migrationId;
            """;
        var migrationIdParameter = checkCommand.CreateParameter();
        migrationIdParameter.ParameterName = "migrationId";
        migrationIdParameter.Value = migrationName;
        checkCommand.Parameters.Add(migrationIdParameter);
        var alreadyApplied = await checkCommand.ExecuteScalarAsync();
        if (alreadyApplied is not null)
            return;

        await using var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = """
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES (@migrationId, @productVersion);
            """;
        var insertMigrationIdParameter = insertCommand.CreateParameter();
        insertMigrationIdParameter.ParameterName = "migrationId";
        insertMigrationIdParameter.Value = migrationName;
        insertCommand.Parameters.Add(insertMigrationIdParameter);

        var productVersionParameter = insertCommand.CreateParameter();
        productVersionParameter.ParameterName = "productVersion";
        productVersionParameter.Value = typeof(TDbContext).Assembly.GetName().Version?.ToString() ?? "unknown";
        insertCommand.Parameters.Add(productVersionParameter);
        await insertCommand.ExecuteNonQueryAsync();
    }

    private static async Task EnsureDatabaseExistsAsync(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        if (string.IsNullOrWhiteSpace(databaseName))
            return;

        builder.Database = "postgres";
        builder.SearchPath = null;

        await using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        await using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @dbName";
        checkCmd.Parameters.AddWithValue("dbName", databaseName);
        var exists = await checkCmd.ExecuteScalarAsync();

        if (exists is null)
        {
            var escapedDb = databaseName.Trim().Replace("\"", "\"\"");
            await using var createCmd = connection.CreateCommand();
            createCmd.CommandText = $"CREATE DATABASE \"{escapedDb}\"";
            await createCmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task EnsureSchemaExistsAsync(string connectionString, string? databaseSchema)
    {
        if (string.IsNullOrWhiteSpace(databaseSchema))
            return;

        var escapedSchema = databaseSchema.Trim().Replace("\"", "\"\"");

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE SCHEMA IF NOT EXISTS \"{escapedSchema}\";";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<bool> IsTenantSchemaReadyAsync(string connectionString, string? databaseSchema)
    {
        if (string.IsNullOrWhiteSpace(databaseSchema))
            return false;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = @schemaName
                      AND table_name = '__EFMigrationsHistory')
                 AND EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = @schemaName
                      AND table_name = 'InvoiceFiscalRecords')
                THEN 1 ELSE 0 END;
            """;
        command.Parameters.AddWithValue("schemaName", databaseSchema.Trim());
        var result = await command.ExecuteScalarAsync();
        return result is not null && Convert.ToInt32(result) == 1;
    }

    private static string BuildTenantKey(AppTenantInfo tenantInfo) =>
        $"{tenantInfo.Id ?? tenantInfo.Identifier}:{tenantInfo.DatabaseName}:{tenantInfo.DatabaseSchema}";
}
