namespace Opplat.Application.Abstractions.Services;

public interface IAuditLogService
{
    Task LogAsync(
        string actorOid,
        string actionType,
        string? targetTenantId = null,
        string? targetUserId = null,
        object? beforeState = null,
        object? afterState = null,
        CancellationToken cancellationToken = default);
}
