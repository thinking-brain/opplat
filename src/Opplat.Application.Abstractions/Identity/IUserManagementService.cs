namespace Opplat.Application.Abstractions.Identity;

/// <summary>
/// High-level user management service abstracting underlying identity provider operations.
/// </summary>
public interface IUserManagementService
{
    /// <summary>
    /// Creates a user in Entra ID. The UPN follows the collision-safe format
    /// <c>{uuid}@{domain}.onmicrosoft.com</c>. Returns the Entra object ID (oid).
    /// </summary>
    Task<UserOperationResult> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);

    /// <summary>Sets <c>accountEnabled = true</c> on the Entra user.</summary>
    Task<UserOperationResult> EnableUserAsync(string objectId, CancellationToken ct = default);

    /// <summary>Sets <c>accountEnabled = false</c> on the Entra user.</summary>
    Task<UserOperationResult> DisableUserAsync(string objectId, CancellationToken ct = default);

    /// <summary>Permanently deletes the Entra user.</summary>
    Task<UserOperationResult> DeleteUserAsync(string objectId, CancellationToken ct = default);

    /// <summary>
    /// Forces a password change on next sign-in
    /// </summary>
    Task<UserOperationResult> ResetPasswordAsync(string objectId, string temporaryPassword, CancellationToken ct = default);

    /// <summary>
    /// Assigns roles to an existing user.
    /// Each role name must match a role that already exists.
    /// </summary>
    Task<UserOperationResult> AssignRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken ct = default);
}

/// <summary>
/// Request to create a new user in the identity provider.
/// </summary>
public sealed record CreateUserRequest
{
    /// <summary>
    /// User's real email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>User's username.</summary>
    public required string UserName { get; init; }

    /// <summary>
    /// First name. Optional, but recommended to populate for better user profile completeness.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Last name. Optional, but recommended to populate for better user profile completeness.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Password for first login. Must meet identity provider complexity requirements.
    /// User could be forced to change on first sign-in.
    /// </summary>
    public required string Password { get; init; }
}

/// <summary>Outcome of a user operation.</summary>
public sealed record UserOperationResult
{
    public bool Succeeded { get; init; }

    /// <summary>
    /// Object ID (oid) — populated on successful create.
    /// </summary>
    public string? ObjectId { get; init; }

    /// <summary>Error detail when <see cref="Succeeded"/> is false.</summary>
    public string? Error { get; init; }

    /// <summary>HTTP status code from Graph API (if applicable).</summary>
    public int? StatusCode { get; init; }

    public static UserOperationResult Success(string? objectId = null) =>
        new() { Succeeded = true, ObjectId = objectId };

    public static UserOperationResult Failure(string error, int? statusCode = null) =>
        new() { Succeeded = false, Error = error, StatusCode = statusCode };
}
