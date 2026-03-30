using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Options;

namespace Opplat.AdminApi.Auth;

public sealed record AuthRuntimeConfiguration(
    string? Authority,
    string? MetadataAddress,
    string? Audience,
    IReadOnlyCollection<string> ValidIssuers,
    string ObjectIdClaimType);

public static class AuthRuntimeConfigurationResolver
{
    public static AuthRuntimeConfiguration Resolve(AuthOptions options)
    {
        if (string.Equals(options.Provider, AuthProviders.EntraId, StringComparison.OrdinalIgnoreCase))
            return ResolveEntra(options);

        var validIssuers = options.ValidIssuers.Count > 0
            ? NormalizeDistinct(options.ValidIssuers)
            : NormalizeDistinct([options.Authority]);

        return new AuthRuntimeConfiguration(
            options.Authority,
            options.MetadataAddress,
            options.Audience,
            validIssuers,
            AuthClaimTypes.ObjectId);
    }

    private static AuthRuntimeConfiguration ResolveEntra(AuthOptions options)
    {
        var authorityHost = options.Entra.AuthorityHost?.Trim().TrimEnd('/');
        var tenantId = options.Entra.TenantId?.Trim();

        if (string.IsNullOrWhiteSpace(options.Authority))
        {
            if (string.IsNullOrWhiteSpace(authorityHost) || string.IsNullOrWhiteSpace(tenantId))
                throw new InvalidOperationException("Auth:Entra:TenantId and Auth:Entra:AuthorityHost must be configured when Auth:Provider is EntraId.");

            options.Authority = $"{authorityHost}/{tenantId}{(options.Entra.UseV2Endpoint ? "/v2.0" : string.Empty)}";
        }

        var validIssuers = options.ValidIssuers.Count > 0
            ? NormalizeDistinct(options.ValidIssuers)
            : BuildDefaultEntraIssuers(authorityHost, tenantId, options.Authority, options.Entra.UseV2Endpoint);

        return new AuthRuntimeConfiguration(
            options.Authority,
            options.MetadataAddress,
            options.Audience,
            validIssuers,
            string.IsNullOrWhiteSpace(options.Entra.ObjectIdClaimType)
                ? AuthClaimTypes.ObjectId
                : options.Entra.ObjectIdClaimType.Trim());
    }

    private static IReadOnlyCollection<string> BuildDefaultEntraIssuers(
        string? authorityHost,
        string? tenantId,
        string? configuredAuthority,
        bool useV2Endpoint)
    {
        var issuers = new List<string>();

        if (!string.IsNullOrWhiteSpace(configuredAuthority))
            issuers.Add(configuredAuthority);

        if (!string.IsNullOrWhiteSpace(authorityHost) && !string.IsNullOrWhiteSpace(tenantId))
        {
            issuers.Add($"{authorityHost}/{tenantId}{(useV2Endpoint ? "/v2.0" : string.Empty)}");
            issuers.Add($"{authorityHost}/{tenantId}/");
            issuers.Add($"https://sts.windows.net/{tenantId}/");
        }

        return NormalizeDistinct(issuers);
    }

    private static IReadOnlyCollection<string> NormalizeDistinct(IEnumerable<string?> values) =>
        values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}
