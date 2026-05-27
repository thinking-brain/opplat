using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Commands;

public record ToggleUserActiveCommand(string UserId) : IRequest<bool>;

public class ToggleUserActiveCommandHandler(
    AdminTenantCatalogDbContext db,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
    IUserManagementService userManagementService,
    ILogger<ToggleUserActiveCommandHandler> logger) : IRequestHandler<ToggleUserActiveCommand, bool>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor = tenantAccessor;
    private readonly IUserManagementService _userManagementService = userManagementService;
    private readonly ILogger<ToggleUserActiveCommandHandler> _logger = logger;

    public async Task<bool> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
    {
        var tenantInfo = _tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
            return false;

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning("ToggleUserActiveCommand: invalid user ID format '{UserId}'.", request.UserId);
            return false;
        }

        var tenantUser = await _db.TenantUsers
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId, cancellationToken);

        if (tenantUser is null)
        {
            _logger.LogWarning("ToggleUserActiveCommand: user {UserId} not found in tenant {TenantId}.", userId, tenantId);
            return false;
        }

        tenantUser.IsActive   = !tenantUser.IsActive;
        tenantUser.ModifiedAt = DateTime.UtcNow;

        if (!tenantUser.IsActive)
            tenantUser.DeactivatedAt = DateTime.UtcNow;

        // Sync with identity provider when EntraOid is available
        if (!string.IsNullOrWhiteSpace(tenantUser.EntraOid))
        {
            var idpResult = tenantUser.IsActive
                ? await _userManagementService.EnableUserAsync(tenantUser.EntraOid, cancellationToken)
                : await _userManagementService.DisableUserAsync(tenantUser.EntraOid, cancellationToken);

            if (!idpResult.Succeeded)
                _logger.LogWarning(
                    "ToggleUserActiveCommand: IdP sync failed for user {UserId} (OID: {Oid}): {Error}.",
                    userId, tenantUser.EntraOid, idpResult.Error);
        }

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Changed status of user {UserId} to {State} in tenant {TenantId}.",
            userId, tenantUser.IsActive, tenantId);

        return true;
    }
}
