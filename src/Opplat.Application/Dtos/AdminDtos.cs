using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Opplat.Application.Dtos;

public sealed class AdminTenantDto
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;

    [JsonPropertyName("databaseSchema")]
    public string DatabaseSchema { get; set; } = string.Empty;

    [JsonPropertyName("schema")]
    public string Schema
    {
        get => DatabaseSchema;
        set => DatabaseSchema = value;
    }

    public int UserCount { get; set; }
    public bool IsActive { get; set; }
}

public sealed class CoreTenantCatalogDto
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string DatabaseSchema { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid SubscriptionPlanId { get; set; }
    public string SubscriptionPlanName { get; set; } = string.Empty;
    public Guid DatabaseInstanceId { get; set; }
    public string DatabaseInstanceIdentifier { get; set; } = string.Empty;
    public int ActiveUserCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? InactivatedAt { get; set; }
}

public sealed class AdminSubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxActiveUsers { get; set; }
    public decimal MaxApiCallsPerMonth { get; set; }
    public decimal MaxStorageGb { get; set; }
    public decimal PricingMonthly { get; set; }
    public string? ResourceLimits { get; set; }
    public bool IsActive { get; set; }
}

public sealed class AdminDatabaseInstanceDto
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public int CurrentTenantSchemaCount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class UpsertTenantRequest
{
    public Guid? Id { get; set; }

    [Required]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string DatabaseName { get; set; } = string.Empty;

    [Required]

    [JsonPropertyName("databaseSchema")]
    public string DatabaseSchema { get; set; } = string.Empty;

    [JsonPropertyName("schema")]
    public string? Schema
    {
        get => string.IsNullOrWhiteSpace(DatabaseSchema) ? null : DatabaseSchema;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                DatabaseSchema = value;
        }
    }

    public bool IsActive { get; set; } = true;
}

public sealed class TenantSchemaMigrationRequest
{
    [Required]
    public string MigrationName { get; set; } = string.Empty;

    [Required]
    public string MigrationSql { get; set; } = string.Empty;

    public string? RollbackSql { get; set; }

    public int? BatchSize { get; set; }

    public int? DelayBetweenBatchesSeconds { get; set; }
}

