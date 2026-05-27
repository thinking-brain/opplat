using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;

namespace Opplat.Infrastructure.Identity;

/// <summary>
/// No-op Graph API stub for local development (Keycloak).
/// Logs a warning and returns success so upstream flows don't fail.
/// </summary>
public sealed class NoOpGraphUserService(ILogger<NoOpGraphUserService> logger) : IUserManagementService
{
    private readonly ILogger<NoOpGraphUserService> _logger = logger;

    public Task<UserOperationResult> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping CreateUser for {Email}", request.Email);
        return Task.FromResult(UserOperationResult.Success($"noop-{Guid.NewGuid():N}"));
    }

    public Task<UserOperationResult> EnableUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping EnableUser for {ObjectId}", objectId);
        return Task.FromResult(UserOperationResult.Success(objectId));
    }

    public Task<UserOperationResult> DisableUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping DisableUser for {ObjectId}", objectId);
        return Task.FromResult(UserOperationResult.Success(objectId));
    }

    public Task<UserOperationResult> DeleteUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping DeleteUser for {ObjectId}", objectId);
        return Task.FromResult(UserOperationResult.Success(objectId));
    }

    public Task<UserOperationResult> ResetPasswordAsync(string objectId, string temporaryPassword, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping ResetPassword for {ObjectId}", objectId);
        return Task.FromResult(UserOperationResult.Success(objectId));
    }

    public Task<UserOperationResult> AssignRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping AssignRoles for {UserId}", userId);
        return Task.FromResult(UserOperationResult.Success(userId));
    }
}
