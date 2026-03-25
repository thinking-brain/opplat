namespace Opplat.MainApp.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string? Authority { get; set; }
    public string? MetadataAddress { get; set; }
    public string? Audience { get; set; }
    public string ClaimNamespace { get; set; } = "https://opplat.com";
    public string AdminRole { get; set; } = AuthRoles.SuperAdmin;
    public string TenantAdminRole { get; set; } = AuthRoles.TenantAdmin;
    public string TenantUserRole { get; set; } = AuthRoles.TenantUser;
    public List<string> AdditionalRoleClaimTypes { get; set; } = new();
    public AdminBffOptions AdminBff { get; set; } = new();
}

public sealed class AdminBffOptions
{
    public string ClientId { get; set; } = "opplat-admin";
    public string? ClientSecret { get; set; }
    public bool ShellModeEnabled { get; set; }
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
    public string DefaultOrigin { get; set; } = string.Empty;
    public List<string> AllowedOrigins { get; set; } = new();
}
