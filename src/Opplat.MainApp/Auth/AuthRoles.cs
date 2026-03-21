namespace Opplat.MainApp.Auth;

public static class AuthRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string TenantAdmin = "TenantAdmin";
    public const string TenantUser = "TenantUser";

    public static readonly string[] TenantAssignable = [TenantAdmin, TenantUser];
}
