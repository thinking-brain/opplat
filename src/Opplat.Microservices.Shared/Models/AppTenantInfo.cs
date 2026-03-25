using Finbuckle.MultiTenant.Abstractions;

namespace Opplat.Microservices.Shared.Models;

public class AppTenantInfo : ITenantInfo
{
    public string? Id { get; set; }
    public string? Identifier { get; set; }
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? DatabaseSchema { get; set; }
    public string? JwtSigningKey { get; set; }
    public bool IsActive { get; set; } = true;
}
