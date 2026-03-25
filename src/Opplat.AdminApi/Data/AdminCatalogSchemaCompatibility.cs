using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Opplat.AdminApi.Data;

internal static partial class AdminCatalogSchemaCompatibility
{
    private const string AdminTenantsTableName = "AdminTenants";

    public static async Task EnsureCurrentSchemaAsync(
        AdminTenantCatalogDbContext db,
        CancellationToken cancellationToken = default)
    {
        if (!db.Database.IsRelational())
            return;

        var connection = db.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;
        if (shouldCloseConnection)
            await connection.OpenAsync(cancellationToken);

        try
        {
            var columns = await GetColumnNamesAsync(connection, cancellationToken);

            if (!columns.Contains("DatabaseName"))
                await db.Database.ExecuteSqlRawAsync(
                    $"ALTER TABLE \"{AdminTenantsTableName}\" ADD COLUMN \"DatabaseName\" character varying(256) NOT NULL DEFAULT '';",
                    cancellationToken);

            if (!columns.Contains("DatabaseSchema"))
                await db.Database.ExecuteSqlRawAsync(
                    $"ALTER TABLE \"{AdminTenantsTableName}\" ADD COLUMN \"DatabaseSchema\" character varying(128) NOT NULL DEFAULT '';",
                    cancellationToken);

            if (!columns.Contains("UserCount"))
                await db.Database.ExecuteSqlRawAsync(
                    $"ALTER TABLE \"{AdminTenantsTableName}\" ADD COLUMN \"UserCount\" integer NOT NULL DEFAULT 0;",
                    cancellationToken);

            columns = await GetColumnNamesAsync(connection, cancellationToken);
            if (!columns.Contains("ConnectionString"))
                return;

            if (!await IsColumnNullableAsync(connection, "ConnectionString", cancellationToken))
            {
                await db.Database.ExecuteSqlRawAsync(
                    $"ALTER TABLE \"{AdminTenantsTableName}\" ALTER COLUMN \"ConnectionString\" DROP NOT NULL;",
                    cancellationToken);
            }

            var updates = await ReadCompatibilityUpdatesAsync(connection, cancellationToken);
            foreach (var update in updates)
                await ApplyCompatibilityUpdateAsync(connection, update, cancellationToken);
        }
        finally
        {
            if (shouldCloseConnection && connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }
    }

    private static async Task<HashSet<string>> GetColumnNamesAsync(
        DbConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            select column_name
            from information_schema.columns
            where table_schema = current_schema()
              and table_name = @tableName;
            """;

        var tableNameParameter = command.CreateParameter();
        tableNameParameter.ParameterName = "@tableName";
        tableNameParameter.Value = AdminTenantsTableName;
        command.Parameters.Add(tableNameParameter);

        var columns = new HashSet<string>(StringComparer.Ordinal);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            columns.Add(reader.GetString(0));

        return columns;
    }

    private static async Task<bool> IsColumnNullableAsync(
        DbConnection connection,
        string columnName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            select is_nullable
            from information_schema.columns
            where table_schema = current_schema()
              and table_name = @tableName
              and column_name = @columnName;
            """;

        var tableNameParameter = command.CreateParameter();
        tableNameParameter.ParameterName = "@tableName";
        tableNameParameter.Value = AdminTenantsTableName;
        command.Parameters.Add(tableNameParameter);

        var columnNameParameter = command.CreateParameter();
        columnNameParameter.ParameterName = "@columnName";
        columnNameParameter.Value = columnName;
        command.Parameters.Add(columnNameParameter);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return string.Equals(result?.ToString(), "YES", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<List<AdminCatalogSchemaUpdate>> ReadCompatibilityUpdatesAsync(
        DbConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            $"""
            select "Id", "Identifier", "ConnectionString", "DatabaseName", "DatabaseSchema", "UserCount"
            from "{AdminTenantsTableName}";
            """;

        var updates = new List<AdminCatalogSchemaUpdate>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetString(0);
            var identifier = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var connectionString = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            var currentDatabaseName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            var currentDatabaseSchema = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
            var currentUserCount = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

            var normalizedIdentifier = NormalizeIdentifier(identifier, id);
            var desiredDatabaseName = string.IsNullOrWhiteSpace(currentDatabaseName)
                ? ResolveDatabaseName(connectionString, normalizedIdentifier)
                : currentDatabaseName;
            var desiredDatabaseSchema = string.IsNullOrWhiteSpace(currentDatabaseSchema)
                ? ResolveDatabaseSchema(normalizedIdentifier)
                : currentDatabaseSchema;
            var desiredUserCount = currentUserCount < 0 ? 0 : currentUserCount;

            if (string.Equals(currentDatabaseName, desiredDatabaseName, StringComparison.Ordinal) &&
                string.Equals(currentDatabaseSchema, desiredDatabaseSchema, StringComparison.Ordinal) &&
                currentUserCount == desiredUserCount)
            {
                continue;
            }

            updates.Add(new AdminCatalogSchemaUpdate(
                id,
                desiredDatabaseName,
                desiredDatabaseSchema,
                desiredUserCount));
        }

        return updates;
    }

    private static async Task ApplyCompatibilityUpdateAsync(
        DbConnection connection,
        AdminCatalogSchemaUpdate update,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            $"""
            UPDATE "{AdminTenantsTableName}"
            SET "DatabaseName" = @databaseName,
                "DatabaseSchema" = @databaseSchema,
                "UserCount" = @userCount
            WHERE "Id" = @id;
            """;

        AddParameter(command, "@databaseName", update.DatabaseName);
        AddParameter(command, "@databaseSchema", update.DatabaseSchema);
        AddParameter(command, "@userCount", update.UserCount);
        AddParameter(command, "@id", update.Id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static string ResolveDatabaseName(string? legacyConnectionString, string normalizedIdentifier)
    {
        if (!string.IsNullOrWhiteSpace(legacyConnectionString))
        {
            if (TryReadConnectionStringDatabaseName(legacyConnectionString, out var databaseName))
                return databaseName;

            try
            {
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(legacyConnectionString);
                if (!string.IsNullOrWhiteSpace(connectionStringBuilder.Database))
                    return connectionStringBuilder.Database.Trim();
            }
            catch (ArgumentException)
            {
            }
        }

        return $"opplat_{normalizedIdentifier}";
    }

    private static bool TryReadConnectionStringDatabaseName(string connectionString, out string databaseName)
    {
        databaseName = string.Empty;

        try
        {
            var connectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            foreach (var key in new[] { "Database", "Initial Catalog" })
            {
                if (connectionStringBuilder.TryGetValue(key, out var value) &&
                    value is string candidate &&
                    !string.IsNullOrWhiteSpace(candidate))
                {
                    databaseName = candidate.Trim();
                    return true;
                }
            }
        }
        catch (ArgumentException)
        {
        }

        return false;
    }

    private static string ResolveDatabaseSchema(string normalizedIdentifier) =>
        $"tenant_{normalizedIdentifier}";

    private static string NormalizeIdentifier(string? identifier, string fallbackId)
    {
        var value = string.IsNullOrWhiteSpace(identifier) ? fallbackId : identifier;
        var normalized = InvalidIdentifierCharactersRegex().Replace(value.Trim().ToLowerInvariant(), "_").Trim('_');
        return string.IsNullOrWhiteSpace(normalized) ? "default" : normalized;
    }

    [GeneratedRegex("[^a-z0-9]+", RegexOptions.Compiled)]
    private static partial Regex InvalidIdentifierCharactersRegex();

    private sealed record AdminCatalogSchemaUpdate(
        string Id,
        string DatabaseName,
        string DatabaseSchema,
        int UserCount);
}
