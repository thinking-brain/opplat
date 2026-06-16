using System.Security.Claims;
using System.Text.Json;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;

namespace Opplat.Api.Admin.Auth;

public static class OidcClaimsNormalizer
{
    public static void Normalize(ClaimsIdentity identity, AuthOptions options)
    {
        NormalizeSimpleClaim(identity, AuthClaimTypes.PreferredUserName, ClaimTypes.Name, options);
        NormalizeSimpleClaim(identity, "email", ClaimTypes.Email, options);
        NormalizeSimpleClaim(identity, AuthClaimTypes.TenantId, AuthClaimTypes.TenantId, options);
        NormalizeSimpleClaim(identity, AuthClaimTypes.TenantIdentifier, AuthClaimTypes.TenantIdentifier, options);
        NormalizeObjectId(identity, options);
        NormalizeRoles(identity, options);
    }

    private static IEnumerable<string> ClaimCandidates(AuthOptions options, string claimType)
    {
        yield return claimType;

        if (!string.IsNullOrWhiteSpace(options.ClaimNamespace))
            yield return $"{options.ClaimNamespace.TrimEnd('/')}/{claimType}";

        if (claimType == ClaimTypes.Name)
            yield return "name";
    }

    private static void NormalizeSimpleClaim(
        ClaimsIdentity identity,
        string sourceClaimType,
        string targetClaimType,
        AuthOptions options)
    {
        if (identity.HasClaim(claim => string.Equals(claim.Type, targetClaimType, StringComparison.OrdinalIgnoreCase)))
            return;

        var sourceClaim = ClaimCandidates(options, sourceClaimType)
            .SelectMany(candidate => identity.FindAll(candidate))
            .FirstOrDefault(claim => !string.IsNullOrWhiteSpace(claim.Value));

        if (sourceClaim is null)
            return;

        identity.AddClaim(new Claim(targetClaimType, sourceClaim.Value));
    }

    private static void NormalizeObjectId(ClaimsIdentity identity, AuthOptions options)
    {
        if (identity.HasClaim(claim => string.Equals(claim.Type, AuthClaimTypes.ObjectId, StringComparison.OrdinalIgnoreCase)))
            return;

        var objectIdClaimType = AuthRuntimeConfigurationResolver.Resolve(options).ObjectIdClaimType;
        var objectId = ClaimCandidates(options, objectIdClaimType)
            .Concat(ClaimCandidates(options, AuthClaimTypes.Subject))
            .SelectMany(candidate => identity.FindAll(candidate))
            .FirstOrDefault(claim => !string.IsNullOrWhiteSpace(claim.Value))
            ?.Value;

        if (!string.IsNullOrWhiteSpace(objectId))
            identity.AddClaim(new Claim(AuthClaimTypes.ObjectId, objectId));
    }

    private static void NormalizeRoles(ClaimsIdentity identity, AuthOptions options)
    {
        var normalizedRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var roleClaimType in GetRoleClaimTypes(options))
        {
            foreach (var claim in identity.FindAll(roleClaimType))
                AddRoles(normalizedRoles, claim.Value);
        }

        foreach (var claim in identity.Claims.Where(claim =>
                     string.Equals(claim.Type, "realm_access", StringComparison.OrdinalIgnoreCase)
                     || string.Equals(claim.Type, $"{options.ClaimNamespace.TrimEnd('/')}/realm_access", StringComparison.OrdinalIgnoreCase)))
        {
            AddRealmAccessRoles(normalizedRoles, claim.Value);
        }

        foreach (var role in normalizedRoles)
        {
            if (!identity.HasClaim(ClaimTypes.Role, role))
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }

    private static IEnumerable<string> GetRoleClaimTypes(AuthOptions options)
    {
        yield return ClaimTypes.Role;
        yield return "roles";
        yield return "role";
        yield return "realm_access.roles";

        foreach (var claimType in options.AdditionalRoleClaimTypes)
            yield return claimType;

        if (!string.IsNullOrWhiteSpace(options.ClaimNamespace))
        {
            yield return $"{options.ClaimNamespace.TrimEnd('/')}/roles";
            yield return $"{options.ClaimNamespace.TrimEnd('/')}/role";
        }
    }

    private static void AddRoles(ISet<string> roles, string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return;

        if (TryParseJsonArray(rawValue, out var jsonRoles))
        {
            foreach (var role in jsonRoles)
                roles.Add(role);

            return;
        }

        foreach (var role in rawValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            roles.Add(role);
    }

    private static void AddRealmAccessRoles(ISet<string> roles, string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return;

        try
        {
            using var json = JsonDocument.Parse(rawValue);
            if (!json.RootElement.TryGetProperty("roles", out var roleArray) || roleArray.ValueKind != JsonValueKind.Array)
                return;

            foreach (var role in roleArray.EnumerateArray().Select(element => element.GetString()).Where(role => !string.IsNullOrWhiteSpace(role)))
                roles.Add(role!);
        }
        catch (JsonException)
        {
        }
    }

    private static bool TryParseJsonArray(string rawValue, out IReadOnlyCollection<string> roles)
    {
        roles = Array.Empty<string>();

        try
        {
            using var json = JsonDocument.Parse(rawValue);
            if (json.RootElement.ValueKind != JsonValueKind.Array)
                return false;

            roles = json.RootElement.EnumerateArray()
                .Select(element => element.GetString())
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Cast<string>()
                .ToArray();

            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
