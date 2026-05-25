using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;
using Opplat.Application.Abstractions.Services;
using Opplat.Application.Dtos.Auth;

namespace Opplat.MainApp;

public sealed class OidcClaimsTransformation(
    IOptions<AuthOptions> options,
    IUserTenantResolver userTenantResolver) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return principal;

        // Provider claim normalization: handles AuthClaimTypes.RealmAccess, AuthClaimTypes.Roles,
        // JsonDocument.Parse payloads, and new Claim(ClaimTypes.Role, role) expansion.
        OidcClaimsNormalizer.Normalize(identity, options.Value);

        // Enrich missing tenant claims from the application's database.
        // This covers users created through the app (e.g. RegisterTenantCommand) that are not
        // seeded in Keycloak with tenant_id/tenant_identifier user attributes, and therefore
        // receive a JWT with no tenant claims at all.
        var hasTenantId = identity.HasClaim(c =>
            c.Type.Equals(AuthClaimTypes.TenantId, StringComparison.OrdinalIgnoreCase));

        if (!hasTenantId)
        {
            var email = identity.Claims.FirstOrDefault(c => c.Type == AuthClaimTypes.Email)?.Value;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var tenantInfo = await userTenantResolver.ResolveAsync(email);
                if (tenantInfo is not null)
                {
                    identity.AddClaim(new Claim(AuthClaimTypes.TenantId, tenantInfo.TenantId));
                    identity.AddClaim(new Claim(AuthClaimTypes.TenantIdentifier, tenantInfo.TenantIdentifier));
                }
            }
        }

        return principal;
    }
}
