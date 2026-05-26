using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Opplat.Application.Abstractions.Options;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services;

/// <summary>
/// Module 3.1: Schema Provisioning Service.
/// Creates dedicated schemas for tenants within assigned database instances.
/// Idempotent — multiple executions for the same tenant are safe.
/// </summary>
public sealed class TenantSchemaProvisioningService
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly TenantDatabaseOptions _tenantDatabaseOptions;
    private readonly ILogger<TenantSchemaProvisioningService> _logger;

    public TenantSchemaProvisioningService(
        AdminTenantCatalogDbContext db,
        TenantDatabaseOptions tenantDatabaseOptions,
        ILogger<TenantSchemaProvisioningService> logger)
    {
        _db = db;
        _tenantDatabaseOptions = tenantDatabaseOptions;
        _logger = logger;
    }

    /// <summary>
    /// Provisions a dedicated schema for the given tenant within its assigned database instance.
    /// Idempotent: if the schema already exists, this operation succeeds without error.
    /// </summary>
    public async Task<TenantSchemaProvisioningOutcome> ProvisionTenantSchemaAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == null)
            throw new ArgumentNullException(nameof(tenant));

        if (string.IsNullOrWhiteSpace(tenant.DatabaseSchema))
            throw new InvalidOperationException($"Tenant {tenant.Id} has no DatabaseSchema assigned.");

        var databaseInstance = await _db.DatabaseInstances
            .FirstOrDefaultAsync(d => d.Id == tenant.DatabaseInstanceId, cancellationToken);

        if (databaseInstance == null)
            throw new InvalidOperationException(
                $"Database instance {tenant.DatabaseInstanceId} not found for tenant {tenant.Id}.");

        var connectionString = BuildConnectionString(databaseInstance.DatabaseName);
        var databaseName = databaseInstance.DatabaseName;
        var databaseCreated = await EnsureDatabaseExistsAsync(connectionString, cancellationToken);
        var result = new TenantSchemaProvisioningOutcome
        {
            TenantId = tenant.Id,
            TenantIdentifier = tenant.Identifier,
            DatabaseInstanceId = databaseInstance.Id,
            DatabaseInstanceIdentifier = databaseInstance.Identifier,
            DatabaseName = databaseName,
            DatabaseSchema = tenant.DatabaseSchema,
            DatabaseCreated = databaseCreated,
            ExecutedAt = DateTime.UtcNow
        };

        _logger.LogInformation(
            "Provisioning schema '{Schema}' for tenant '{TenantId}' in database instance '{InstanceId}'.",
            tenant.DatabaseSchema, tenant.Id, databaseInstance.Id);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        try
        {
            // Check if schema already exists (idempotent).
            var schemaExists = await SchemaExistsAsync(connection, tenant.DatabaseSchema, cancellationToken);
            if (schemaExists)
            {
                _logger.LogInformation(
                    "Schema '{Schema}' already exists for tenant '{TenantId}'. Skipping creation (idempotent).",
                    tenant.DatabaseSchema, tenant.Id);
                result.AlreadyProvisioned = true;
                result.Succeeded = true;
            }
            else
            {
                // Create schema with AUTHORIZATION clause so tenant owns it.
                var createSchemaCommand = $"CREATE SCHEMA \"{tenant.DatabaseSchema}\" AUTHORIZATION postgres;";
                await using var command = connection.CreateCommand();
                command.CommandText = createSchemaCommand;
                await command.ExecuteNonQueryAsync(cancellationToken);

                _logger.LogInformation(
                    "Schema '{Schema}' successfully created for tenant '{TenantId}'.",
                    tenant.DatabaseSchema, tenant.Id);
                result.SchemaCreated = true;
                result.Succeeded = true;
            }
        }
        finally
        {
            await connection.CloseAsync();
        }

        if (result.Succeeded)
        {
            var migrationConnectionString = BuildConnectionString(databaseInstance.DatabaseName, tenant.DatabaseSchema);
            await RunEfMigrationsAsync(migrationConnectionString, tenant.DatabaseSchema, cancellationToken);
            result.MigrationsApplied = true;
        }

        return result;
    }

    public async Task<bool> SchemaExistsAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == null)
            throw new ArgumentNullException(nameof(tenant));

        if (string.IsNullOrWhiteSpace(tenant.DatabaseSchema))
            return false;

        var databaseInstance = await _db.DatabaseInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == tenant.DatabaseInstanceId, cancellationToken);

        if (databaseInstance == null || string.IsNullOrWhiteSpace(databaseInstance.DatabaseName))
            return false;

        var connectionString = BuildConnectionString(databaseInstance.DatabaseName);
        if (!await DatabaseExistsAsync(connectionString, cancellationToken))
            return false;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return await SchemaExistsAsync(connection, tenant.DatabaseSchema, cancellationToken);
    }

    /// <summary>
    /// Drops a tenant's schema (cleanup/offboarding). Non-idempotent — fails if schema doesn't exist.
    /// </summary>
    public async Task DropTenantSchemaAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == null)
            throw new ArgumentNullException(nameof(tenant));

        if (string.IsNullOrWhiteSpace(tenant.DatabaseSchema))
            throw new InvalidOperationException($"Tenant {tenant.Id} has no DatabaseSchema assigned.");

        var databaseInstance = await _db.DatabaseInstances
            .FirstOrDefaultAsync(d => d.Id == tenant.DatabaseInstanceId, cancellationToken);

        if (databaseInstance == null)
            throw new InvalidOperationException(
                $"Database instance {tenant.DatabaseInstanceId} not found for tenant {tenant.Id}.");

        _logger.LogInformation(
            "Dropping schema '{Schema}' for tenant '{TenantId}' from database instance '{InstanceId}'.",
            tenant.DatabaseSchema, tenant.Id, databaseInstance.Id);

        var connectionString = BuildConnectionString(databaseInstance.DatabaseName);
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        try
        {
            var dropSchemaCommand = $"DROP SCHEMA IF EXISTS \"{tenant.DatabaseSchema}\" CASCADE;";
            await using var command = connection.CreateCommand();
            command.CommandText = dropSchemaCommand;
            await command.ExecuteNonQueryAsync(cancellationToken);

            _logger.LogInformation(
                "Schema '{Schema}' successfully dropped for tenant '{TenantId}'.",
                tenant.DatabaseSchema, tenant.Id);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static async Task<bool> SchemaExistsAsync(
        DbConnection connection,
        string schemaName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT 1 FROM information_schema.schemata 
            WHERE schema_name = @schemaName;
            """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@schemaName";
        parameter.Value = schemaName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result != null && (int)result == 1;
    }

    private async Task RunEfMigrationsAsync(string connectionString, string schemaName, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Running EF Core migrations for tenant schema '{Schema}'.",
            schemaName);

        var inventoryOptions = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var inventoryDb = new InventoryDbContext(inventoryOptions, null);
        await inventoryDb.Database.MigrateAsync(cancellationToken);

        var salesOptions = new DbContextOptionsBuilder<SalesDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var salesDb = new SalesDbContext(salesOptions, null);
        await salesDb.Database.MigrateAsync(cancellationToken);

        var opplatOptions = new DbContextOptionsBuilder<OpplatDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var opplatDb = new OpplatDbContext(opplatOptions, null);
        await opplatDb.Database.MigrateAsync(cancellationToken);

        _logger.LogInformation(
            "EF Core migrations completed for tenant schema '{Schema}'.",
            schemaName);
    }

    private string BuildConnectionString(string databaseName, string? schema = null)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = _tenantDatabaseOptions.Host,
            Port = _tenantDatabaseOptions.Port,
            Username = _tenantDatabaseOptions.Username,
            Password = _tenantDatabaseOptions.Password,
            Database = databaseName,
            SslMode = SslMode.Disable
        };

        if (!string.IsNullOrWhiteSpace(schema))
            builder.SearchPath = schema;

        return builder.ConnectionString;
    }

    private static string NormalizeConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;
        return builder.ConnectionString;
    }

    private static async Task<bool> EnsureDatabaseExistsAsync(string connectionString, CancellationToken cancellationToken)
    {
        if (await DatabaseExistsAsync(connectionString, cancellationToken))
            return false;

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("A database name is required for tenant schema provisioning.");

        var escapedDatabaseName = databaseName.Replace("\"", "\"\"");
        await using var adminConnection = new NpgsqlConnection(BuildAdminConnectionString(connectionString));
        await adminConnection.OpenAsync(cancellationToken);

        try
        {
            await using var createDatabase = adminConnection.CreateCommand();
            createDatabase.CommandText = $"CREATE DATABASE \"{escapedDatabaseName}\";";
            await createDatabase.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (PostgresException ex) when (string.Equals(ex.SqlState, PostgresErrorCodes.DuplicateDatabase, StringComparison.Ordinal))
        {
            return false;
        }
    }

    private static async Task<bool> DatabaseExistsAsync(string connectionString, CancellationToken cancellationToken)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;
        if (string.IsNullOrWhiteSpace(databaseName))
            return false;

        await using var adminConnection = new NpgsqlConnection(BuildAdminConnectionString(connectionString));
        await adminConnection.OpenAsync(cancellationToken);
        await using var command = adminConnection.CreateCommand();
        command.CommandText =
            """
            SELECT 1
            FROM pg_database
            WHERE datname = @databaseName;
            """;
        command.Parameters.AddWithValue("databaseName", databaseName);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }

    private static string BuildAdminConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres",
            SearchPath = string.Empty
        };

        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;

        return builder.ConnectionString;
    }

    private static string ReadDatabaseName(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        return builder.Database ?? string.Empty;
    }
}

public sealed class TenantSchemaProvisioningOutcome
{
    public required Guid TenantId { get; set; }
    public required string TenantIdentifier { get; set; }
    public required string DatabaseSchema { get; set; }
    public required string DatabaseName { get; set; }
    public required string DatabaseInstanceIdentifier { get; set; }
    public Guid DatabaseInstanceId { get; set; }
    public bool Succeeded { get; set; }
    public bool AlreadyProvisioned { get; set; }
    public bool DatabaseCreated { get; set; }
    public bool SchemaCreated { get; set; }
    public bool MigrationsApplied { get; set; }
    public DateTime ExecutedAt { get; set; }
}
