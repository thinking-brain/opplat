namespace Opplat.MainApp.Auth;

public static class AuthRoles
{
    public const string SuperAdmin = Opplat.Application.Abstractions.Auth.AuthRoles.SuperAdmin;
    public const string TenantAdmin = Opplat.Application.Abstractions.Auth.AuthRoles.TenantAdmin;
    public const string TenantUser = Opplat.Application.Abstractions.Auth.AuthRoles.TenantUser;

    public static readonly string[] TenantAssignable = Opplat.Application.Abstractions.Auth.AuthRoles.TenantAssignable;
}
