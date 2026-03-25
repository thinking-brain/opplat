namespace Opplat.Application.Abstractions.Auth;

public static class AuthClaimTypes
{
    public const string TenantId = "tenant_id";
    public const string TenantIdentifier = "tenant_identifier";
    public const string RealmAccess = "realm_access";
    public const string RealmAccessRoles = "realm_access.roles";
    public const string Roles = "roles";
    public const string PreferredUserName = "preferred_username";

    /// <summary>
    /// Entra ID stable object identifier. Used as the primary user key for Graph API operations.
    /// Keycloak equivalent: "sub".
    /// </summary>
    public const string ObjectId = "oid";

    /// <summary>
    /// Standard OIDC subject identifier. Stable across both Keycloak and Entra ID.
    /// </summary>
    public const string Subject = "sub";
}
