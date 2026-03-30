using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Options;

namespace Opplat.AdminApi.Auth;

public sealed class OidcClaimsTransformation : IClaimsTransformation
{
    private readonly IOptions<AuthOptions> _options;

    public OidcClaimsTransformation(IOptions<AuthOptions> options)
    {
        _options = options;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is ClaimsIdentity identity)
            OidcClaimsNormalizer.Normalize(identity, _options.Value);

        return Task.FromResult(principal);
    }
}
