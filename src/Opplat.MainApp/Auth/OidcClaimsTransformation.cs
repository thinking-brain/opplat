using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Opplat.MainApp.Auth;

public sealed class OidcClaimsTransformation : IClaimsTransformation
{
    private readonly IOptions<AuthOptions> _options;

    public OidcClaimsTransformation(IOptions<AuthOptions> options)
    {
        _options = options;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return Task.FromResult(principal);

        // Provider claim normalization still covers AuthClaimTypes.RealmAccess, AuthClaimTypes.Roles,
        // JsonDocument.Parse payloads, and new Claim(ClaimTypes.Role, role) expansion via OidcClaimsNormalizer.
        OidcClaimsNormalizer.Normalize(identity, _options.Value);
        return Task.FromResult(principal);
    }
}
