namespace Opplat.Application.Dtos;

public sealed class TenantAccessContextDto
{
    public bool IsResolved { get; set; }

    public bool IsActive { get; set; }

    public string Status { get; set; } = "unresolved";

    public string? TenantId { get; set; }

    public string? TenantIdentifier { get; set; }

    public string? TenantName { get; set; }

    public string Message { get; set; } = string.Empty;
}
