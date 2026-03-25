namespace Opplat.Application.Abstractions.Identity;

/// <summary>
/// Abstracts Microsoft Graph API user-lifecycle operations.
/// Implementation uses client-credentials flow (service principal, not delegated).
/// In local-dev (Keycloak) a no-op or mock implementation may be registered instead.
/// </summary>
public interface IGraphUserService
{
    /// <summary>
    /// Creates a user in Entra ID. The UPN follows the collision-safe format
    /// <c>{uuid}@{domain}.onmicrosoft.com</c>. Returns the Entra object ID (oid).
    /// </summary>
    Task<GraphUserResult> CreateUserAsync(CreateGraphUserRequest request, CancellationToken ct = default);

    /// <summary>Sets <c>accountEnabled = true</c> on the Entra user.</summary>
    Task<GraphUserResult> EnableUserAsync(string objectId, CancellationToken ct = default);

    /// <summary>Sets <c>accountEnabled = false</c> on the Entra user.</summary>
    Task<GraphUserResult> DisableUserAsync(string objectId, CancellationToken ct = default);

    /// <summary>Permanently deletes the Entra user.</summary>
    Task<GraphUserResult> DeleteUserAsync(string objectId, CancellationToken ct = default);

    /// <summary>
    /// Forces a password change on next sign-in via
    /// <c>passwordProfile.forceChangePasswordNextSignIn = true</c>.
    /// </summary>
    Task<GraphUserResult> ResetPasswordAsync(string objectId, string temporaryPassword, CancellationToken ct = default);
}

/// <summary>Request to create a new Entra ID user via Graph API.</summary>
public sealed record CreateGraphUserRequest
{
    /// <summary>User's real email address. Stored in <c>mail</c> or <c>otherMails</c>.</summary>
    public required string Email { get; init; }

    /// <summary>Display name for the user profile.</summary>
    public required string DisplayName { get; init; }

    /// <summary>Given (first) name.</summary>
    public string? GivenName { get; init; }

    /// <summary>Surname (last name).</summary>
    public string? Surname { get; init; }

    /// <summary>
    /// Temporary password for first login. Must meet Entra complexity requirements.
    /// User will be forced to change on first sign-in.
    /// </summary>
    public required string TemporaryPassword { get; init; }
}

/// <summary>Outcome of a Graph API user operation.</summary>
public sealed record GraphUserResult
{
    public bool Succeeded { get; init; }

    /// <summary>Entra object ID (oid) — populated on successful create.</summary>
    public string? ObjectId { get; init; }

    /// <summary>Error detail when <see cref="Succeeded"/> is false.</summary>
    public string? Error { get; init; }

    /// <summary>HTTP status code from Graph API (if applicable).</summary>
    public int? StatusCode { get; init; }

    public static GraphUserResult Success(string? objectId = null) =>
        new() { Succeeded = true, ObjectId = objectId };

    public static GraphUserResult Failure(string error, int? statusCode = null) =>
        new() { Succeeded = false, Error = error, StatusCode = statusCode };
}
