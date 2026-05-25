namespace Opplat.Application.Abstractions.Services;

/// <summary>
/// Tenant information resolved from the application's database for a given external user identity.
/// </summary>
/// <param name="TenantId">The tenant's internal GUID identifier.</param>
/// <param name="TenantIdentifier">The tenant's URL-safe slug (e.g. "mojocafe").</param>
public sealed record UserTenantInfo(string TenantId, string TenantIdentifier);

/// <summary>
/// Resolves tenant information from the application's database using an external user identity (Keycloak sub / Entra OID).
/// Used to enrich JWT claims for users created through the application rather than seeded with Keycloak attributes.
/// </summary>
public interface IUserTenantResolver
{
    /// <summary>
    /// Returns the tenant association for the user with the given email,
    /// or <c>null</c> if the user has no tenant association (e.g. global administrators).
    /// </summary>
    Task<UserTenantInfo?> ResolveAsync(string userEmail, CancellationToken ct = default);
}
