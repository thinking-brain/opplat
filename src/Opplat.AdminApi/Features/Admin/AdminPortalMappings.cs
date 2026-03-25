using Opplat.AdminApi.Endpoints;
using Opplat.AdminApi.Models;

namespace Opplat.AdminApi.Features.Admin;

internal static class AdminPortalMappings
{
    internal static AdminTenantDto ToDto(AdminTenantInfo tenant) => new()
    {
        Id = tenant.Id,
        Identifier = tenant.Identifier,
        Name = tenant.Name,
        DatabaseName = tenant.DatabaseName,
        DatabaseSchema = tenant.DatabaseSchema,
        UserCount = tenant.UserCount,
        IsActive = tenant.IsActive
    };

    internal static string NormalizeTenantIdentifier(string? identifier)
    {
        var normalized = identifier?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Identifier is required.");

        return normalized;
    }

    internal static string NormalizeTenantName(string? name)
    {
        var normalized = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Name is required.");

        return normalized;
    }

    internal static string NormalizeDatabaseName(string? databaseName)
    {
        var normalized = databaseName?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Database name is required.");

        return normalized;
    }

    internal static string NormalizeDatabaseSchema(string? databaseSchema)
    {
        var normalized = databaseSchema?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Database schema is required.");

        return normalized;
    }
}
