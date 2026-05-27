using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;

namespace Opplat.Infrastructure.Identity;

/// <summary>
/// No-op Keycloak stub for environments where Keycloak is not configured.
/// Logs a warning and returns success so upstream flows don't fail.
/// </summary>
public sealed class NoOpKeycloakUserService(ILogger<NoOpKeycloakUserService> logger) : IUserManagementService
{
    private readonly ILogger<NoOpKeycloakUserService> _logger = logger;

    public Task<UserOperationResult> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping CreateUser for {Email}", request.Email);
        return Task.FromResult(UserOperationResult.Success($"noop-{Guid.NewGuid():N}"));
    }

    public Task<UserOperationResult> DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping DeleteUser for {UserId}", userId);
        return Task.FromResult(UserOperationResult.Success(userId));
    }

    public Task<UserOperationResult> AssignRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping AssignRealmRoles [{Roles}] for {UserId}",
            string.Join(", ", roleNames), userId);
        return Task.FromResult(UserOperationResult.Success(userId));
    }

    public Task<UserOperationResult> DisableUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping DisableUser for {ObjectId}", objectId);
        return Task.FromResult(UserOperationResult.Success(objectId));
    }

    public Task<UserOperationResult> ResetPasswordAsync(string objectId, string temporaryPassword, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping ResetPassword for {ObjectId}", objectId);
        return Task.FromResult(UserOperationResult.Success(objectId));
    }

    public Task<UserOperationResult> EnableUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping EnableUser for {ObjectId}", objectId);
        return Task.FromResult(UserOperationResult.Success(objectId));
    }
}
