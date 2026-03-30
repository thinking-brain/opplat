namespace Opplat.Infrastructure.Identity;

/// <summary>
/// Configuration for the Keycloak Admin REST API client.
/// Bind from the <c>Keycloak</c> configuration section.
/// </summary>
public sealed class KeycloakAdminOptions
{
    public const string SectionName = "Keycloak";

    /// <summary>Keycloak base URL (e.g., http://localhost:8180).</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Realm to manage users in (e.g., opplat).</summary>
    public string Realm { get; set; } = string.Empty;

    /// <summary>Admin username (master realm admin).</summary>
    public string AdminUsername { get; set; } = string.Empty;

    /// <summary>Admin password (master realm admin).</summary>
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>When false, a no-op stub is registered instead of the real service.</summary>
    public bool Enabled { get; set; } = false;
}
