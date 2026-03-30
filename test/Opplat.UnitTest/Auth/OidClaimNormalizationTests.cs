using System.Security.Claims;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;
using Opplat.MainApp;

namespace Opplat.UnitTest.Auth;

public class OidClaimNormalizationTests
{
    [Fact]
    public async Task TransformAsync_NormalizesEntraOidClaim_WhenPresent()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(new Claim("oid", "entra-object-id-123"));

        await transformation.TransformAsync(principal);

        var identity = (ClaimsIdentity)principal.Identity!;
        Assert.Equal("entra-object-id-123", identity.FindFirst(AuthClaimTypes.ObjectId)?.Value);
    }

    [Fact]
    public async Task TransformAsync_FallsBackToSubClaim_WhenNoOidPresent()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(new Claim("sub", "keycloak-subject-456"));

        await transformation.TransformAsync(principal);

        var identity = (ClaimsIdentity)principal.Identity!;
        Assert.Equal("keycloak-subject-456", identity.FindFirst(AuthClaimTypes.ObjectId)?.Value);
    }

    [Fact]
    public async Task TransformAsync_PreservesExistingOidClaim_WhenBothPresent()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(
            new Claim("oid", "entra-oid"),
            new Claim("sub", "entra-sub"));

        await transformation.TransformAsync(principal);

        var identity = (ClaimsIdentity)principal.Identity!;
        var oidClaims = identity.FindAll(AuthClaimTypes.ObjectId).Select(c => c.Value).ToList();

        Assert.Single(oidClaims);
        Assert.Equal("entra-oid", oidClaims[0]);
    }

    [Fact]
    public async Task TransformAsync_DoesNotAddOid_WhenNeitherPresent()
    {
        var transformation = CreateTransformation();
        var principal = CreatePrincipal(new Claim("preferred_username", "someuser"));

        await transformation.TransformAsync(principal);

        var identity = (ClaimsIdentity)principal.Identity!;
        Assert.Null(identity.FindFirst(AuthClaimTypes.ObjectId));
    }

    [Fact]
    public void AuthClaimTypes_DefinesObjectIdAndSubject()
    {
        Assert.Equal("oid", AuthClaimTypes.ObjectId);
        Assert.Equal("sub", AuthClaimTypes.Subject);
    }

    [Fact]
    public void Source_OidcClaimsNormalizerCallsNormalizeObjectId()
    {
        var source = TestRepository.ReadAllText("src", "Opplat.MainApp", "Auth", "OidcClaimsNormalizer.cs");

        Assert.Contains("NormalizeObjectId", source);
        Assert.Contains("AuthClaimTypes.ObjectId", source);
        Assert.Contains("AuthClaimTypes.Subject", source);
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
