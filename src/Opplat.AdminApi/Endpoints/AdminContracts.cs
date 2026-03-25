using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Opplat.Application.Abstractions.Admin;

namespace Opplat.AdminApi.Endpoints;

public sealed class AdminTenantDto
{
    public string Id { get; set; } = string.Empty;
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

public sealed class UpsertTenantRequest
{
    public string? Id { get; set; }

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

