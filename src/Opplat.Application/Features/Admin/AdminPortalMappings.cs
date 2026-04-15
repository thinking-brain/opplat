using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;

namespace Opplat.Application.Features.Admin;

internal static class AdminPortalMappings
{
    internal static AdminTenantDto ToDto(Tenant tenant) => new()
    {
        Id = tenant.Id,
        Identifier = tenant.Identifier,
        Name = tenant.Name,
        DatabaseSchema = tenant.DatabaseSchema,
        UserCount = tenant.TenantUsers.Count,
        IsActive = tenant.Status == TenantStatus.Active
    };

    internal static CoreTenantCatalogDto ToCoreDto(Tenant tenant, int activeUserCount) => new()
    {
        Id = tenant.Id,
        Identifier = tenant.Identifier,
        Name = tenant.Name,
        DatabaseName = tenant.DatabaseInstance?.DatabaseName ?? string.Empty,
        DatabaseSchema = tenant.DatabaseSchema ?? string.Empty,
        Status = tenant.Status.ToString(),
        SubscriptionPlanId = tenant.SubscriptionPlanId,
        SubscriptionPlanName = tenant.SubscriptionPlan?.Name ?? string.Empty,
        DatabaseInstanceId = tenant.DatabaseInstanceId,
        DatabaseInstanceIdentifier = tenant.DatabaseInstance?.Identifier ?? string.Empty,
        ActiveUserCount = activeUserCount,
        CreatedAt = tenant.CreatedAt,
        InactivatedAt = tenant.InactivatedAt
    };

    internal static AdminSubscriptionPlanDto ToDto(SubscriptionPlan plan) => new()
    {
        Id = plan.Id,
        Name = plan.Name,
        Description = plan.Description,
        MaxActiveUsers = plan.MaxActiveUsers,
        MaxApiCallsPerMonth = plan.MaxApiCallsPerMonth,
        MaxStorageGb = plan.MaxStorageGb,
        PricingMonthly = plan.PricingMonthly,
        ResourceLimits = plan.ResourceLimits,
        IsActive = plan.IsActive
    };

    internal static AdminDatabaseInstanceDto ToDto(DatabaseInstance instance) => new()
    {
        Id = instance.Id,
        Identifier = instance.Identifier,
        DatabaseName = instance.DatabaseName,
        CurrentTenantSchemaCount = instance.CurrentTenantSchemaCount,
        Status = instance.Status.ToString()
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
