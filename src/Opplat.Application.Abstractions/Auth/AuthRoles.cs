namespace Opplat.Application.Abstractions.Auth;

public static class AuthRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string TenantAdmin = "TenantAdmin";
    public const string TenantUser = "TenantUser";

    /// <summary>Can sell, take payments and open/close their own shift; also granted to integration clients.</summary>
    public const string PosCashier = "PosCashier";

    /// <summary>Can additionally manage registers and payment terminals.</summary>
    public const string PosManager = "PosManager";

    public const string PosOperatorPolicy = "PosOperator";
    public const string PosManagerPolicy = "PosManager";

    public static readonly string[] TenantAssignable = [TenantAdmin, TenantUser];
}
