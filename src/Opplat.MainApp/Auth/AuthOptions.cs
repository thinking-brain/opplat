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
}
