using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;

namespace Opplat.Infrastructure.Identity;

/// <summary>
/// No-op Keycloak stub for environments where Keycloak is not configured.
/// Logs a warning and returns success so upstream flows don't fail.
/// </summary>
public sealed class NoOpKeycloakUserService(ILogger<NoOpKeycloakUserService> logger) : IKeycloakUserService
{
    private readonly ILogger<NoOpKeycloakUserService> _logger = logger;

    public Task<KeycloakUserResult> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping CreateUser for {Email}", request.Email);
        return Task.FromResult(KeycloakUserResult.Success($"noop-{Guid.NewGuid():N}"));
    }

    public Task<KeycloakUserResult> DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        _logger.LogWarning("Keycloak disabled — skipping DeleteUser for {UserId}", userId);
        return Task.FromResult(KeycloakUserResult.Success(userId));
    }
}
