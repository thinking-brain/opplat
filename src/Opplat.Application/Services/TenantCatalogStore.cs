using System.Text.Json;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Opplat.Domain.Models;

namespace Opplat.Application.Services;

public sealed class TenantCatalogStore : IMultiTenantStore<AppTenantInfo>
{
    private const string CentralCatalogCacheKey = "tenant-catalog:central";

    private readonly string _catalogPath;
    private readonly string? _adminCatalogConnectionString;
    private readonly TimeSpan _cacheLifetime;
    private readonly List<AppTenantInfo> _seedTenants;
    private readonly IMemoryCache _memoryCache;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public TenantCatalogStore(IConfiguration configuration, IHostEnvironment hostEnvironment, IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _seedTenants = configuration
            .GetSection("Finbuckle:MultiTenant:Stores:ConfigurationStore:Tenants")
            .Get<List<AppTenantInfo>>() ?? new List<AppTenantInfo>();
        _adminCatalogConnectionString = configuration.GetConnectionString("AdminCatalogConnection");
        _cacheLifetime = TimeSpan.FromSeconds(Math.Max(5, configuration.GetValue("TenantStore:CatalogCacheSeconds", 30)));

        var relativePath = configuration["TenantStore:CatalogPath"];
        _catalogPath = Path.IsPathRooted(relativePath ?? string.Empty)
            ? relativePath!
            : Path.Combine(hostEnvironment.ContentRootPath, relativePath ?? Path.Combine("Data", "tenant-catalog.json"));

        EnsureCatalogExists();
    }

    public async Task<bool> TryAddAsync(AppTenantInfo tenantInfo)
    {
        ArgumentNullException.ThrowIfNull(tenantInfo);

        if (CanUseCentralCatalog())
            return await TryAddCentralAsync(tenantInfo);

        await _gate.WaitAsync();
        try
        {
            var catalog = await ReadCatalogAsync();
            if (catalog.Tenants.Any(existing =>
                    string.Equals(existing.Identifier, tenantInfo.Identifier, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(tenantInfo.Id) &&
                     string.Equals(existing.Id, tenantInfo.Id, StringComparison.OrdinalIgnoreCase))))
            {
                return false;
            }

            catalog.Tenants.Add(CloneTenant(tenantInfo));
            await WriteCatalogAsync(catalog);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> TryUpdateAsync(AppTenantInfo tenantInfo)
    {
        ArgumentNullException.ThrowIfNull(tenantInfo);

        if (CanUseCentralCatalog())
            return await TryUpdateCentralAsync(tenantInfo);

        await _gate.WaitAsync();
        try
        {
            var catalog = await ReadCatalogAsync();
            var index = catalog.Tenants.FindIndex(existing =>
                (!string.IsNullOrWhiteSpace(tenantInfo.Id) &&
                 string.Equals(existing.Id, tenantInfo.Id, StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(existing.Identifier, tenantInfo.Identifier, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
                return false;

            catalog.Tenants[index] = CloneTenant(tenantInfo);
            await WriteCatalogAsync(catalog);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> TryRemoveAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        if (CanUseCentralCatalog())
            return await TryRemoveCentralAsync(identifier);

        await _gate.WaitAsync();
        try
        {
            var catalog = await ReadCatalogAsync();
            var removed = catalog.Tenants.RemoveAll(existing =>
                string.Equals(existing.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

            if (removed == 0)
                return false;

            await WriteCatalogAsync(catalog);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<AppTenantInfo?> TryGetByIdentifierAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return null;

        if (CanUseCentralCatalog())
        {
            var centralCatalog = await TryReadCentralCatalogAsync();
            if (centralCatalog is not null)
            {
                var cachedMatch = centralCatalog
                    .Where(tenant => tenant.IsActive)
                    .Select(CloneTenant)
                    .FirstOrDefault(tenant => string.Equals(tenant.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

                if (cachedMatch is not null)
                    return cachedMatch;

                var directMatch = await TryReadCentralTenantByIdentifierAsync(identifier);
                if (directMatch is not null)
                    return CloneTenant(directMatch);
            }
        }

        var catalog = await ReadCatalogAsync();
        return catalog.Tenants
            .Where(tenant => tenant.IsActive)
            .Select(CloneTenant)
            .FirstOrDefault(tenant => string.Equals(tenant.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<AppTenantInfo?> TryGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        if (CanUseCentralCatalog())
        {
            var centralCatalog = await TryReadCentralCatalogAsync();
            if (centralCatalog is not null)
            {
                var cachedMatch = centralCatalog
                    .Where(tenant => tenant.IsActive)
                    .Select(CloneTenant)
                    .FirstOrDefault(tenant => string.Equals(tenant.Id, id, StringComparison.OrdinalIgnoreCase));

                if (cachedMatch is not null)
                    return cachedMatch;

                var directMatch = await TryReadCentralTenantByIdAsync(id);
                if (directMatch is not null)
                    return CloneTenant(directMatch);
            }
        }

        var catalog = await ReadCatalogAsync();
        return catalog.Tenants
            .Where(tenant => tenant.IsActive)
            .Select(CloneTenant)
            .FirstOrDefault(tenant => string.Equals(tenant.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<AppTenantInfo>> GetAllAsync()
    {
        if (CanUseCentralCatalog())
        {
            var centralCatalog = await TryReadCentralCatalogAsync();
            if (centralCatalog is not null)
                return centralCatalog.Select(CloneTenant).ToList();
        }

        var catalog = await ReadCatalogAsync();
        return catalog.Tenants.Select(CloneTenant).ToList();
    }

    private bool CanUseCentralCatalog() => !string.IsNullOrWhiteSpace(_adminCatalogConnectionString);

    private void EnsureCatalogExists()
    {
        var directory = Path.GetDirectoryName(_catalogPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(_catalogPath))
            return;

        var initialCatalog = new TenantCatalogDocument
        {
            Tenants = _seedTenants.Select(CloneTenant).ToList()
        };

        var json = JsonSerializer.Serialize(initialCatalog, _jsonOptions);
        File.WriteAllText(_catalogPath, json);
    }

    private async Task<TenantCatalogDocument> ReadCatalogAsync()
    {
        await using var stream = File.Open(_catalogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var catalog = await JsonSerializer.DeserializeAsync<TenantCatalogDocument>(stream, _jsonOptions);

        if (catalog?.Tenants is { Count: > 0 })
            return catalog;

        return new TenantCatalogDocument
        {
            Tenants = _seedTenants.Select(CloneTenant).ToList()
        };
    }

    private async Task WriteCatalogAsync(TenantCatalogDocument catalog)
    {
        await using var stream = File.Open(_catalogPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, catalog, _jsonOptions);
    }

    private async Task<List<AppTenantInfo>?> TryReadCentralCatalogAsync()
    {
        if (_memoryCache.TryGetValue(CentralCatalogCacheKey, out List<AppTenantInfo>? cachedCatalog) && cachedCatalog is not null)
            return cachedCatalog.Select(CloneTenant).ToList();

        try
        {
            var catalog = await ReadCentralCatalogAsync();
            _memoryCache.Set(CentralCatalogCacheKey, catalog, _cacheLifetime);
            return catalog.Select(CloneTenant).ToList();
        }
        catch (NpgsqlException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private async Task<List<AppTenantInfo>> ReadCentralCatalogAsync()
    {
        await using var connection = new NpgsqlConnection(_adminCatalogConnectionString);
        await connection.OpenAsync();

        const string sql =
            """
            select t."Id",
                   t."Identifier",
                   t."Name",
                   t."Status",
                   t."DatabaseSchema",
                   di."ConnectionStringReference"
            from tenants t
            inner join database_instances di on di."Id" = t."DatabaseInstanceId"
            order by t."Name", t."Identifier";
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        var catalog = new List<AppTenantInfo>();
        while (await reader.ReadAsync())
            catalog.Add(ReadTenant(reader));

        return catalog;
    }

    private async Task<AppTenantInfo?> TryReadCentralTenantByIdentifierAsync(string identifier)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_adminCatalogConnectionString);
            await connection.OpenAsync();

            const string sql =
                """
                select t."Id",
                       t."Identifier",
                       t."Name",
                       t."Status",
                       t."DatabaseSchema",
                       di."ConnectionStringReference"
                from tenants t
                inner join database_instances di on di."Id" = t."DatabaseInstanceId"
                where lower(t."Identifier") = lower(@identifier)
                limit 1;
                """;

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("identifier", identifier.Trim());
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return ReadTenant(reader);
        }
        catch (NpgsqlException)
        {
            return null;
        }
    }

    private async Task<AppTenantInfo?> TryReadCentralTenantByIdAsync(string id)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_adminCatalogConnectionString);
            await connection.OpenAsync();

            const string sql =
                """
                select t."Id",
                       t."Identifier",
                       t."Name",
                       t."Status",
                       t."DatabaseSchema",
                       di."ConnectionStringReference"
                from tenants t
                inner join database_instances di on di."Id" = t."DatabaseInstanceId"
                where t."Id" = @id
                limit 1;
                """;

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", id.Trim());
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return ReadTenant(reader);
        }
        catch (NpgsqlException)
        {
            return null;
        }
    }

    private async Task<bool> TryAddCentralAsync(AppTenantInfo tenantInfo)
    {
        await _gate.WaitAsync();
        try
        {
            await using var connection = new NpgsqlConnection(_adminCatalogConnectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            if (await TenantExistsAsync(connection, transaction, tenantInfo, null))
                return false;

            var identifier = NormalizeIdentifier(tenantInfo.Identifier);
            var tenantId = string.IsNullOrWhiteSpace(tenantInfo.Id) ? $"tenant-{Guid.NewGuid():N}" : tenantInfo.Id.Trim();
            var databaseSchema = NormalizeDatabaseSchema(tenantInfo.DatabaseSchema, identifier);
            var databaseInstanceId = await ResolveDatabaseInstanceIdAsync(connection, transaction, tenantInfo, identifier);
            var planId = await ResolveSubscriptionPlanIdAsync(connection, transaction);

            const string insertSql =
                """
                insert into tenants ("Id", "Identifier", "Name", "Status", "SubscriptionPlanId", "CreatedAt", "DatabaseInstanceId", "DatabaseSchema")
                values (@id, @identifier, @name, @status, @subscriptionPlanId, @createdAt, @databaseInstanceId, @databaseSchema);
                """;

            await using var command = new NpgsqlCommand(insertSql, connection, transaction);
            command.Parameters.AddWithValue("id", tenantId);
            command.Parameters.AddWithValue("identifier", identifier);
            command.Parameters.AddWithValue("name", tenantInfo.Name?.Trim() ?? identifier);
            command.Parameters.AddWithValue("status", tenantInfo.IsActive ? "Active" : "Inactive");
            command.Parameters.AddWithValue("subscriptionPlanId", planId);
            command.Parameters.AddWithValue("createdAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("databaseInstanceId", databaseInstanceId);
            command.Parameters.AddWithValue("databaseSchema", databaseSchema);
            await command.ExecuteNonQueryAsync();

            await RecalculateDatabaseInstanceCountAsync(connection, transaction, databaseInstanceId);
            await transaction.CommitAsync();
            _memoryCache.Remove(CentralCatalogCacheKey);
            return true;
        }
        catch (NpgsqlException)
        {
            return false;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<bool> TryUpdateCentralAsync(AppTenantInfo tenantInfo)
    {
        await _gate.WaitAsync();
        try
        {
            await using var connection = new NpgsqlConnection(_adminCatalogConnectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            var existing = await LoadExistingTenantAsync(connection, transaction, tenantInfo);
            if (existing is null)
                return false;

            if (await TenantExistsAsync(connection, transaction, tenantInfo, existing.Value.Id))
                return false;

            var identifier = NormalizeIdentifier(tenantInfo.Identifier);
            var databaseSchema = NormalizeDatabaseSchema(tenantInfo.DatabaseSchema, identifier);
            var databaseInstanceId = await ResolveDatabaseInstanceIdAsync(connection, transaction, tenantInfo, identifier);

            const string updateSql =
                """
                update tenants
                set "Identifier" = @identifier,
                    "Name" = @name,
                    "Status" = @status,
                    "DatabaseInstanceId" = @databaseInstanceId,
                    "DatabaseSchema" = @databaseSchema,
                    "InactivatedAt" = @inactivatedAt
                where "Id" = @id;
                """;

            await using var command = new NpgsqlCommand(updateSql, connection, transaction);
            command.Parameters.AddWithValue("id", existing.Value.Id);
            command.Parameters.AddWithValue("identifier", identifier);
            command.Parameters.AddWithValue("name", tenantInfo.Name?.Trim() ?? existing.Value.Name);
            command.Parameters.AddWithValue("status", tenantInfo.IsActive ? "Active" : "Inactive");
            command.Parameters.AddWithValue("databaseInstanceId", databaseInstanceId);
            command.Parameters.AddWithValue("databaseSchema", databaseSchema);
            command.Parameters.AddWithValue("inactivatedAt", tenantInfo.IsActive ? DBNull.Value : DateTime.UtcNow);
            await command.ExecuteNonQueryAsync();

            await RecalculateDatabaseInstanceCountAsync(connection, transaction, existing.Value.DatabaseInstanceId);
            if (existing.Value.DatabaseInstanceId != databaseInstanceId)
                await RecalculateDatabaseInstanceCountAsync(connection, transaction, databaseInstanceId);

            await transaction.CommitAsync();
            _memoryCache.Remove(CentralCatalogCacheKey);
            return true;
        }
        catch (NpgsqlException)
        {
            return false;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<bool> TryRemoveCentralAsync(string identifier)
    {
        await _gate.WaitAsync();
        try
        {
            await using var connection = new NpgsqlConnection(_adminCatalogConnectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            const string selectSql =
                """
                select "Id", "DatabaseInstanceId"
                from tenants
                where lower("Identifier") = lower(@identifier)
                limit 1;
                """;

            await using var selectCommand = new NpgsqlCommand(selectSql, connection, transaction);
            selectCommand.Parameters.AddWithValue("identifier", identifier.Trim());
            await using var reader = await selectCommand.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return false;

            var tenantId = reader.GetString(0);
            var databaseInstanceId = reader.GetInt32(1);
            await reader.CloseAsync();

            const string updateSql =
                """
                update tenants
                set "Status" = 'Inactive',
                    "InactivatedAt" = @inactivatedAt
                where "Id" = @id;
                """;

            await using var updateCommand = new NpgsqlCommand(updateSql, connection, transaction);
            updateCommand.Parameters.AddWithValue("id", tenantId);
            updateCommand.Parameters.AddWithValue("inactivatedAt", DateTime.UtcNow);
            await updateCommand.ExecuteNonQueryAsync();

            await RecalculateDatabaseInstanceCountAsync(connection, transaction, databaseInstanceId);
            await transaction.CommitAsync();
            _memoryCache.Remove(CentralCatalogCacheKey);
            return true;
        }
        catch (NpgsqlException)
        {
            return false;
        }
        finally
        {
            _gate.Release();
        }
    }

    private static async Task<bool> TenantExistsAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        AppTenantInfo tenantInfo,
        string? excludedTenantId)
    {
        const string sql =
            """
            select count(*)
            from tenants
            where lower("Identifier") = lower(@identifier)
              and (@excludedTenantId is null or "Id" <> @excludedTenantId);
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("identifier", NormalizeIdentifier(tenantInfo.Identifier));
        command.Parameters.AddWithValue("excludedTenantId", string.IsNullOrWhiteSpace(excludedTenantId) ? DBNull.Value : excludedTenantId);
        return Convert.ToInt64(await command.ExecuteScalarAsync()) > 0;
    }

    private static async Task<(string Id, string Name, int DatabaseInstanceId)?> LoadExistingTenantAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        AppTenantInfo tenantInfo)
    {
        const string sql =
            """
            select "Id", "Name", "DatabaseInstanceId"
            from tenants
            where (@id is not null and "Id" = @id)
               or lower("Identifier") = lower(@identifier)
            limit 1;
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("id", string.IsNullOrWhiteSpace(tenantInfo.Id) ? DBNull.Value : tenantInfo.Id.Trim());
        command.Parameters.AddWithValue("identifier", NormalizeIdentifier(tenantInfo.Identifier));
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return (reader.GetString(0), reader.GetString(1), reader.GetInt32(2));
    }

    private static async Task<int> ResolveSubscriptionPlanIdAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        const string sql =
            """
            select "Id"
            from subscription_plans
            where "IsActive" = true
            order by "Id"
            limit 1;
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        var result = await command.ExecuteScalarAsync();
        if (result is null || result is DBNull)
            throw new InvalidOperationException("No active subscription plan exists in the central catalog.");

        return Convert.ToInt32(result);
    }

    private static async Task<int> ResolveDatabaseInstanceIdAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        AppTenantInfo tenantInfo,
        string identifier)
    {
        var databaseName = ResolveDatabaseName(tenantInfo, identifier);
        var existingInstanceId = await FindDatabaseInstanceIdAsync(connection, transaction, databaseName);
        if (existingInstanceId.HasValue)
            return existingInstanceId.Value;

        var connectionStringReference = ResolveConnectionStringReference(tenantInfo, databaseName);
        const string insertSql =
            """
            insert into database_instances ("Identifier", "ConnectionStringReference", "CurrentTenantSchemaCount", "Status", "CreatedAt")
            values (@identifier, @connectionStringReference, 0, 'Active', @createdAt)
            returning "Id";
            """;

        await using var command = new NpgsqlCommand(insertSql, connection, transaction);
        command.Parameters.AddWithValue("identifier", $"db-{NormalizeIdentifier(databaseName)}");
        command.Parameters.AddWithValue("connectionStringReference", connectionStringReference);
        command.Parameters.AddWithValue("createdAt", DateTime.UtcNow);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static async Task<int?> FindDatabaseInstanceIdAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string databaseName)
    {
        const string sql =
            """
            select "Id", "ConnectionStringReference"
            from database_instances
            order by "Id";
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var candidateDatabaseName = ReadDatabaseName(reader.GetString(1));
            if (string.Equals(candidateDatabaseName, databaseName, StringComparison.OrdinalIgnoreCase))
                return reader.GetInt32(0);
        }

        return null;
    }

    private static async Task RecalculateDatabaseInstanceCountAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        int databaseInstanceId)
    {
        const string sql =
            """
            update database_instances di
            set "CurrentTenantSchemaCount" = (
                select count(*)
                from tenants t
                where t."DatabaseInstanceId" = di."Id")
            where di."Id" = @databaseInstanceId;
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("databaseInstanceId", databaseInstanceId);
        await command.ExecuteNonQueryAsync();
    }

    private static string ResolveDatabaseName(AppTenantInfo tenantInfo, string identifier)
    {
        if (!string.IsNullOrWhiteSpace(tenantInfo.DatabaseName))
            return tenantInfo.DatabaseName.Trim();

        var databaseNameFromConnection = ReadDatabaseName(tenantInfo.ConnectionString);
        return string.IsNullOrWhiteSpace(databaseNameFromConnection)
            ? $"opplat_{identifier}"
            : databaseNameFromConnection;
    }

    private static string ResolveConnectionStringReference(AppTenantInfo tenantInfo, string databaseName)
    {
        var baseConnectionString = !string.IsNullOrWhiteSpace(tenantInfo.ConnectionString)
            ? tenantInfo.ConnectionString!
            : $"Host=localhost;Port=5432;Username=postgres;Password=Admin123*;Database={databaseName}";

        var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Database = databaseName,
            SearchPath = null
        };

        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;

        return builder.ConnectionString;
    }

    private static string BuildTenantConnectionString(string connectionStringReference, string? databaseSchema)
    {
        if (string.IsNullOrWhiteSpace(connectionStringReference))
            return string.Empty;

        var builder = new NpgsqlConnectionStringBuilder(connectionStringReference);
        if (builder.SslMode == SslMode.Prefer)
            builder.SslMode = SslMode.Disable;

        if (!string.IsNullOrWhiteSpace(databaseSchema))
            builder.SearchPath = databaseSchema.Trim();

        return builder.ConnectionString;
    }

    private static string ReadDatabaseName(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;

        try
        {
            return new NpgsqlConnectionStringBuilder(connectionString).Database ?? string.Empty;
        }
        catch (ArgumentException)
        {
            return string.Empty;
        }
    }

    private static string NormalizeIdentifier(string? identifier) =>
        string.IsNullOrWhiteSpace(identifier) ? "default" : identifier.Trim().ToLowerInvariant();

    private static string NormalizeDatabaseSchema(string? databaseSchema, string identifier) =>
        string.IsNullOrWhiteSpace(databaseSchema) ? $"tenant_{identifier}" : databaseSchema.Trim();

    private static AppTenantInfo CloneTenant(AppTenantInfo tenant) => new()
    {
        Id = tenant.Id,
        Identifier = tenant.Identifier,
        Name = tenant.Name,
        ConnectionString = tenant.ConnectionString,
        DatabaseName = tenant.DatabaseName,
        DatabaseSchema = tenant.DatabaseSchema,
        JwtSigningKey = tenant.JwtSigningKey,
        IsActive = tenant.IsActive
    };

    private static AppTenantInfo ReadTenant(NpgsqlDataReader reader)
    {
        var connectionString = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
        var databaseSchema = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);

        return new AppTenantInfo
        {
            Id = reader.GetString(0),
            Identifier = reader.GetString(1),
            Name = reader.GetString(2),
            ConnectionString = BuildTenantConnectionString(connectionString, databaseSchema),
            DatabaseName = ReadDatabaseName(connectionString),
            DatabaseSchema = databaseSchema,
            IsActive = string.Equals(reader.GetString(3), "Active", StringComparison.OrdinalIgnoreCase)
        };
    }

    public Task<bool> AddAsync(AppTenantInfo tenantInfo)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(AppTenantInfo tenantInfo)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(string identifier)
    {
        throw new NotImplementedException();
    }

    public Task<AppTenantInfo?> GetByIdentifierAsync(string identifier)
    {
        throw new NotImplementedException();
    }

    public Task<AppTenantInfo?> GetAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppTenantInfo>> GetAllAsync(int take, int skip)
    {
        throw new NotImplementedException();
    }

    private sealed class TenantCatalogDocument
    {
        public List<AppTenantInfo> Tenants { get; set; } = new();
    }
}
