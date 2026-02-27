using Finbuckle.MultiTenant.Abstractions;

namespace Opplat.MainApp.Models;

/// <summary>
/// Tenant information for Opplat multi-tenant system.
/// </summary>
public class AppTenantInfo : ITenantInfo
{
    /// <summary>
    /// Unique identifier (GUID) - used internally
    /// </summary>
    public string? Id { get; set; }
    
    /// <summary>
    /// URL-safe identifier (e.g., "mojocafe", "restaurant-a")
    /// Used in route: /{__tenant__}/api/...
    /// </summary>
    public string? Identifier { get; set; }
    
    /// <summary>
    /// Display name (e.g., "Mojo Café")
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Per-tenant connection string (full database isolation)
    /// </summary>
    public string? ConnectionString { get; set; }
    
    /// <summary>
    /// JWT signing key (optional per-tenant override)
    /// Falls back to global config if null.
    /// </summary>
    public string? JwtSigningKey { get; set; }
    
    /// <summary>
    /// Whether the tenant is active (for soft-disable)
    /// </summary>
    public bool IsActive { get; set; } = true;
}
