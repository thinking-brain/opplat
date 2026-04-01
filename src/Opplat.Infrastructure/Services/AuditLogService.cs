using System.Text.Json;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Services;
using Opplat.Domain.Entities.Administration;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services;

public sealed class AuditLogService : IAuditLogService
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(AdminTenantCatalogDbContext db, ILogger<AuditLogService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task LogAsync(
        string actorOid,
        string actionType,
        string? targetTenantId = null,
        string? targetUserId = null,
        object? beforeState = null,
        object? afterState = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entry = new AuditLog
            {
                ActorOid = actorOid,
                ActionType = actionType,
                TargetTenantId = targetTenantId,
                TargetTenantIdFk = targetTenantId,
                TargetUserId = targetUserId,
                BeforeState = beforeState is null ? null : JsonSerializer.Serialize(beforeState),
                AfterState = afterState is null ? null : JsonSerializer.Serialize(afterState),
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            _db.AuditLogs.Add(entry);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Audit log failures must not fail the business operation
            _logger.LogError(ex, "Failed to write audit log for action {ActionType} by {ActorOid}", actionType, actorOid);
        }
    }
}
