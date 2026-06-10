using System.Security.Claims;
using Microsoft.Extensions.Options;
using Opplat.Api.Admin.Auth;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;

namespace Opplat.UnitTest.Auth;

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
    public async Task TransformAsync_AddsStableObjectIdFromSubjectWhenOidIsMissing()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(new Claim(AuthClaimTypes.Subject, "entra-oid-123"));

        await transformation.TransformAsync(principal);

        var objectIds = principal.FindAll(AuthClaimTypes.ObjectId).Select(claim => claim.Value).ToList();

        Assert.Single(objectIds);
        Assert.Equal("entra-oid-123", objectIds[0]);
    }

    [Fact]
    public async Task TransformAsync_PreservesExistingObjectIdWithoutDuplicates()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(
            new Claim(AuthClaimTypes.ObjectId, "existing-oid"),
            new Claim(AuthClaimTypes.Subject, "subject-oid"));

        await transformation.TransformAsync(principal);

        var objectIds = principal.FindAll(AuthClaimTypes.ObjectId).Select(claim => claim.Value).ToList();

        Assert.Single(objectIds);
        Assert.Equal("existing-oid", objectIds[0]);
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
        var source = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Auth", "OidcClaimsTransformation.cs");

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

    [Fact]
    public async Task TransformAsync_PreservesExistingTenantIdentifierClaimsWithoutDuplicates()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(
            new Claim(AuthClaimTypes.TenantIdentifier, "mojocafe"),
            new Claim("https://opplat.com/tenant_identifier", "demo"));

        await transformation.TransformAsync(principal);

        var tenantIdentifiers = principal.FindAll(AuthClaimTypes.TenantIdentifier).Select(claim => claim.Value).ToList();

        Assert.Single(tenantIdentifiers);
        Assert.Equal("mojocafe", tenantIdentifiers[0]);
    }

    [Fact]
    public void Source_NormalizesTenantClaimsThroughSharedNormalizer()
    {
        var source = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Auth", "OidcClaimsNormalizer.cs");

        Assert.Contains("NormalizeClaim(identity, AuthClaimTypes.TenantId", source);
        Assert.Contains("NormalizeClaim(identity, AuthClaimTypes.TenantIdentifier", source);
        Assert.Contains("identity.AddClaim(new Claim(targetClaimType, claimValue));", source);
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
}
