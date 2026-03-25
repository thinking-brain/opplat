using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;

namespace Opplat.Infrastructure.Identity;

/// <summary>
/// No-op Graph API stub for local development (Keycloak).
/// Logs a warning and returns success so upstream flows don't fail.
/// </summary>
public sealed class NoOpGraphUserService : IGraphUserService
{
    private readonly ILogger<NoOpGraphUserService> _logger;

    public NoOpGraphUserService(ILogger<NoOpGraphUserService> logger)
    {
        _logger = logger;
    }

    public Task<GraphUserResult> CreateUserAsync(CreateGraphUserRequest request, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping CreateUser for {Email}", request.Email);
        return Task.FromResult(GraphUserResult.Success($"noop-{Guid.NewGuid():N}"));
    }

    public Task<GraphUserResult> EnableUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping EnableUser for {ObjectId}", objectId);
        return Task.FromResult(GraphUserResult.Success(objectId));
    }

    public Task<GraphUserResult> DisableUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping DisableUser for {ObjectId}", objectId);
        return Task.FromResult(GraphUserResult.Success(objectId));
    }

    public Task<GraphUserResult> DeleteUserAsync(string objectId, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping DeleteUser for {ObjectId}", objectId);
        return Task.FromResult(GraphUserResult.Success(objectId));
    }

    public Task<GraphUserResult> ResetPasswordAsync(string objectId, string temporaryPassword, CancellationToken ct = default)
    {
        _logger.LogWarning("Graph API disabled — skipping ResetPassword for {ObjectId}", objectId);
        return Task.FromResult(GraphUserResult.Success(objectId));
    }
}
