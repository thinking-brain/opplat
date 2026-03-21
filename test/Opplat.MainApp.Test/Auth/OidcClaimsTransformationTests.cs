using System.IO;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Opplat.MainApp.Auth;

namespace Opplat.MainApp.Test.Auth;

public class OidcClaimsTransformationTests
{
    [Fact]
    public async Task TransformAsync_NormalizesNamespacedTenantClaims()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(
            new Claim("https://opplat.com/tenant_id", "tenant-a"),
            new Claim("https://opplat.com/tenant_identifier", "mojocafe"));

        await transformation.TransformAsync(principal);

        var identity = (ClaimsIdentity)principal.Identity!;

        Assert.Equal("tenant-a", identity.FindFirst(AuthClaimTypes.TenantId)?.Value);
        Assert.Equal("mojocafe", identity.FindFirst(AuthClaimTypes.TenantIdentifier)?.Value);
    }

    [Fact]
    public async Task TransformAsync_AddsNameClaimFromPreferredUsername()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(new Claim(AuthClaimTypes.PreferredUserName, "bishop"));

        await transformation.TransformAsync(principal);

        Assert.Equal("bishop", principal.FindFirst(ClaimTypes.Name)?.Value);
    }

    [Fact]
    public async Task TransformAsync_MapsRealmAccessRolesIntoRoleClaims()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(new Claim(AuthClaimTypes.RealmAccess, """{"roles":["SuperAdmin","TenantAdmin"]}"""));

        await transformation.TransformAsync(principal);

        var roles = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();

        Assert.Contains("SuperAdmin", roles);
        Assert.Contains("TenantAdmin", roles);
    }

    [Fact]
    public async Task TransformAsync_MapsJsonArrayRoleClaimsWithoutDuplicates()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(
            new Claim(AuthClaimTypes.Roles, """["TenantAdmin","TenantUser"]"""),
            new Claim(ClaimTypes.Role, "TenantAdmin"));

        await transformation.TransformAsync(principal);

        var roles = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();

        Assert.Equal(2, roles.Count);
        Assert.Contains("TenantAdmin", roles);
        Assert.Contains("TenantUser", roles);
    }

    [Fact]
    public async Task TransformAsync_IgnoresMalformedRealmAccessPayload()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(new Claim(AuthClaimTypes.RealmAccess, "{not-json"));

        await transformation.TransformAsync(principal);

        Assert.Empty(principal.FindAll(ClaimTypes.Role));
    }

    [Fact]
    public void Source_MapsRolesFromProviderSpecificClaims()
    {
        var source = File.ReadAllText(ResolveRepoFile("src", "Opplat.MainApp", "Auth", "OidcClaimsTransformation.cs"));

        Assert.Contains("AuthClaimTypes.RealmAccess", source);
        Assert.Contains("AuthClaimTypes.Roles", source);
        Assert.Contains("new Claim(ClaimTypes.Role, role)", source);
        Assert.Contains("JsonDocument.Parse", source);
    }

    [Fact]
    public async Task TransformAsync_PreservesExistingTenantClaimsWithoutDuplicates()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(
            new Claim(AuthClaimTypes.TenantId, "tenant-a"),
            new Claim("https://opplat.com/tenant_id", "tenant-b"));

        await transformation.TransformAsync(principal);

        var tenantClaims = principal.FindAll(AuthClaimTypes.TenantId).Select(claim => claim.Value).ToList();

        Assert.Single(tenantClaims);
        Assert.Equal("tenant-a", tenantClaims[0]);
    }

    private static OidcClaimsTransformation CreateTransformation()
    {
        return new OidcClaimsTransformation(Options.Create(new AuthOptions
        {
            ClaimNamespace = "https://opplat.com"
        }));
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "Bearer"));
    }

    private static string ResolveRepoFile(params string[] segments)
    {
        var current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "opplat.sln")))
            {
                return Path.Combine(new[] { current }.Concat(segments).ToArray());
            }

            current = Directory.GetParent(current)?.FullName!;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }
}
