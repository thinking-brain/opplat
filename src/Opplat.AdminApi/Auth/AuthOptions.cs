using Opplat.Application.Abstractions.Auth;

namespace Opplat.AdminApi.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>
    /// Selects the identity-provider shape for the live admin auth entry point.
    /// Use <c>EntraId</c> for Azure Entra ID and override to <c>Keycloak</c> locally when needed.
    /// </summary>
    public string Provider { get; set; } = AuthProviders.EntraId;
    public string? Authority { get; set; }
    public string? MetadataAddress { get; set; }
    public string? Audience { get; set; }
    public List<string> ValidIssuers { get; set; } = new();
    public string ClaimNamespace { get; set; } = "https://opplat.com";
    public string AdminRole { get; set; } = AuthRoles.SuperAdmin;
    public string TenantAdminRole { get; set; } = AuthRoles.TenantAdmin;
    public string TenantUserRole { get; set; } = AuthRoles.TenantUser;
    public List<string> AdditionalRoleClaimTypes { get; set; } = new();
    public EntraAuthOptions Entra { get; set; } = new();
    public AdminBffOptions AdminBff { get; set; } = new();
}

public static class AuthProviders
{
    public const string EntraId = "EntraId";
    public const string Keycloak = "Keycloak";
    public const string GenericOidc = "GenericOidc";
}

public sealed class EntraAuthOptions
{
    /// <summary>
    /// Azure Entra tenant ID (directory ID) from the manual portal app-registration step.
    /// MFA and SSPR remain portal/policy configuration and are not enforced by runtime code.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Login host for Entra ID. Defaults to the global public cloud endpoint.
    /// </summary>
    public string AuthorityHost { get; set; } = "https://login.microsoftonline.com";

    /// <summary>
    /// Uses the v2.0 OpenID Connect endpoint. Keep enabled unless a legacy tenant requires otherwise.
    /// </summary>
    public bool UseV2Endpoint { get; set; } = true;

    /// <summary>
    /// Claim type carrying the stable Entra object identifier. Defaults to <c>oid</c>.
    /// </summary>
    public string ObjectIdClaimType { get; set; } = AuthClaimTypes.ObjectId;
}

public sealed class AdminBffOptions
{
    public string ClientId { get; set; } = "opplat-admin";
    public string? ClientSecret { get; set; }
    public bool ShellModeEnabled { get; set; } = true;
    public List<string> Scopes { get; set; } = ["openid", "profile", "email"];
    public bool UsePkce { get; set; } = true;
    public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;
    public bool UseAudienceQueryParam { get; set; }
    public string LoginPath { get; set; } = "/auth/bff/admin/login";
    public string LogoutPath { get; set; } = "/auth/bff/admin/logout";
    public string CallbackPath { get; set; } = "/signin-oidc-admin";
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc-admin";
    public string AccessDeniedPath { get; set; } = "/auth/bff/admin/access-denied";
    public string SessionCookieName { get; set; } = "opplat.admin.session";
    public string CsrfCookieName { get; set; } = "opplat.admin.csrf";
    public string CsrfHeaderName { get; set; } = "X-Opplat-CSRF";
    public int SessionHours { get; set; } = 8;
    public string DefaultOrigin { get; set; } = "http://localhost:3201";
    public List<string> AllowedOrigins { get; set; } =
    [
        "http://localhost:3001",
        "http://localhost:3101",
        "http://localhost:3201",
        "http://localhost:5174"
    ];
}
