namespace Opplat.Application.Abstractions.Identity;

/// <summary>
/// Abstracts Keycloak Admin REST API user-lifecycle operations.
/// Used in local-dev and cloud environments where Keycloak is the OIDC provider.
/// </summary>
public interface IKeycloakUserService
{
    /// <summary>
    /// Creates a user in the configured Keycloak realm.
    /// Returns the Keycloak user ID on success.
    /// </summary>
    Task<KeycloakUserResult> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken ct = default);

    /// <summary>Permanently deletes the Keycloak user.</summary>
    Task<KeycloakUserResult> DeleteUserAsync(string userId, CancellationToken ct = default);
}

/// <summary>Request to create a new Keycloak user.</summary>
public sealed record CreateKeycloakUserRequest
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Password { get; init; }
    public bool Enabled { get; init; } = true;
    public bool EmailVerified { get; init; } = false;
}

/// <summary>Outcome of a Keycloak Admin API user operation.</summary>
public sealed record KeycloakUserResult
{
    public bool Succeeded { get; init; }
    public string? UserId { get; init; }
    public string? ErrorMessage { get; init; }

    public static KeycloakUserResult Success(string? userId = null) =>
        new() { Succeeded = true, UserId = userId };

    public static KeycloakUserResult Failure(string errorMessage) =>
        new() { Succeeded = false, ErrorMessage = errorMessage };
}
