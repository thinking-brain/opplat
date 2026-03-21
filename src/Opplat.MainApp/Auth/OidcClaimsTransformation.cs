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

        var options = _options.Value;
        NormalizeClaim(identity, AuthClaimTypes.TenantId, ClaimCandidates(options, AuthClaimTypes.TenantId));
        NormalizeClaim(identity, AuthClaimTypes.TenantIdentifier, ClaimCandidates(options, AuthClaimTypes.TenantIdentifier));
        NormalizeName(identity);
        NormalizeRoles(identity, options);

        return Task.FromResult(principal);
    }

    private static IEnumerable<string> ClaimCandidates(AuthOptions options, string claimType)
    {
        yield return claimType;

        var claimNamespace = options.ClaimNamespace?.TrimEnd('/');
        if (!string.IsNullOrWhiteSpace(claimNamespace))
            yield return $"{claimNamespace}/{claimType}";
    }

    private static void NormalizeClaim(ClaimsIdentity identity, string targetClaimType, IEnumerable<string> sourceClaimTypes)
    {
        if (identity.HasClaim(claim => string.Equals(claim.Type, targetClaimType, StringComparison.OrdinalIgnoreCase)))
            return;

        var claimValue = sourceClaimTypes
            .Select(sourceClaimType => identity.FindFirst(sourceClaimType)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        if (!string.IsNullOrWhiteSpace(claimValue))
            identity.AddClaim(new Claim(targetClaimType, claimValue));
    }

    private static void NormalizeName(ClaimsIdentity identity)
    {
        if (identity.HasClaim(claim => claim.Type == ClaimTypes.Name))
            return;

        var name = identity.FindFirst(AuthClaimTypes.PreferredUserName)?.Value
            ?? identity.FindFirst("name")?.Value
            ?? identity.FindFirst("unique_name")?.Value;

        if (!string.IsNullOrWhiteSpace(name))
            identity.AddClaim(new Claim(ClaimTypes.Name, name));
    }

    private static void NormalizeRoles(ClaimsIdentity identity, AuthOptions options)
    {
        var existingRoles = new HashSet<string>(
            identity.FindAll(ClaimTypes.Role).Select(claim => claim.Value),
            StringComparer.OrdinalIgnoreCase);

        var claimTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "role",
            AuthClaimTypes.Roles,
            AuthClaimTypes.RealmAccessRoles
        };

        foreach (var claimType in options.AdditionalRoleClaimTypes)
        {
            if (!string.IsNullOrWhiteSpace(claimType))
                claimTypes.Add(claimType);
        }

        var claimNamespace = options.ClaimNamespace?.TrimEnd('/');
        if (!string.IsNullOrWhiteSpace(claimNamespace))
            claimTypes.Add($"{claimNamespace}/{AuthClaimTypes.Roles}");

        foreach (var claimType in claimTypes)
        {
            foreach (var claim in identity.FindAll(claimType).ToList())
            {
                foreach (var role in ExpandClaimValues(claim.Value))
                {
                    if (existingRoles.Add(role))
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }

        foreach (var claim in identity.Claims
                     .Where(claim => claim.Type.StartsWith("resource_access.", StringComparison.OrdinalIgnoreCase)
                         && claim.Type.EndsWith(".roles", StringComparison.OrdinalIgnoreCase))
                     .ToList())
        {
            foreach (var role in ExpandClaimValues(claim.Value))
            {
                if (existingRoles.Add(role))
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }

        var realmAccessClaim = identity.FindFirst(AuthClaimTypes.RealmAccess)?.Value;
        if (string.IsNullOrWhiteSpace(realmAccessClaim))
            return;

        try
        {
            using var document = JsonDocument.Parse(realmAccessClaim);
            if (!document.RootElement.TryGetProperty(AuthClaimTypes.Roles, out var rolesElement) ||
                rolesElement.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var roleElement in rolesElement.EnumerateArray())
            {
                var role = roleElement.GetString();
                if (!string.IsNullOrWhiteSpace(role) && existingRoles.Add(role))
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }
        catch (JsonException)
        {
            // Ignore malformed provider role payloads and continue with any already-mapped roles.
        }
    }

    private static IEnumerable<string> ExpandClaimValues(string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimValue))
            return Array.Empty<string>();

        if (claimValue.StartsWith("[", StringComparison.Ordinal))
        {
            try
            {
                using var document = JsonDocument.Parse(claimValue);
                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return document.RootElement
                        .EnumerateArray()
                        .Select(item => item.GetString())
                        .Where(arrayValue => !string.IsNullOrWhiteSpace(arrayValue))!
                        .Cast<string>()
                        .ToList();
                }
            }
            catch (JsonException)
            {
                // Fall back to treating the claim as a single value.
            }
        }

        return new[] { claimValue };
    }
}
