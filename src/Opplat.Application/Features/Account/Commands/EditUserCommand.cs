using Finbuckle.MultiTenant.Abstractions;
using Opplat.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Commands;

public record EditUserCommand(string Id, string Name, string LastName) : IRequest<bool>;

public sealed record UpdateOwnProfileCommand(string ExternalUserId, string Name, string LastName) : IRequest<bool>;

public class EditUserCommandHandler : IRequestHandler<EditUserCommand, bool>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor;
    private readonly ILogger<EditUserCommandHandler> _logger;
    private readonly IUserManagementService _userManagementService;

    public EditUserCommandHandler(
        AdminTenantCatalogDbContext db,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
        ILogger<EditUserCommandHandler> logger,
        IUserManagementService userManagementService)
    {
        _db             = db;
        _tenantAccessor = tenantAccessor;
        _logger         = logger;
        _userManagementService = userManagementService;
    }

    public async Task<bool> Handle(EditUserCommand request, CancellationToken cancellationToken)
    {
        var tenantInfo = _tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
            return false;

        if (!Guid.TryParse(request.Id, out var userId))
        {
            _logger.LogWarning("EditUserCommand: invalid user ID format '{Id}'.", request.Id);
            return false;
        }

        var tenantUser = await _db.TenantUsers
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId, cancellationToken);

        if (tenantUser is null)
        {
            _logger.LogWarning("EditUserCommand: user {UserId} not found in tenant {TenantId}.", userId, tenantId);
            return false;
        }

        var result = await _userManagementService.UpdateProfileAsync(
            tenantUser.EntraOid ?? tenantUser.Id.ToString(), request.Name, request.LastName, cancellationToken);
        if (!result.Succeeded)
        {
            _logger.LogWarning("EditUserCommand: profile update failed for user {UserId} in tenant {TenantId}.", userId, tenantId);
        }

        return result.Succeeded;
    }
}

public sealed class UpdateOwnProfileCommandHandler(
    AdminTenantCatalogDbContext db,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
    Opplat.Application.Abstractions.Identity.IUserManagementService userManagementService)
    : IRequestHandler<UpdateOwnProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateOwnProfileCommand request, CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
            return false;

        var tenantUser = await db.TenantUsers.FirstOrDefaultAsync(
            user => user.TenantId == tenantId && user.EntraOid == request.ExternalUserId,
            cancellationToken);
        if (tenantUser is null)
            return false;

        var result = await userManagementService.UpdateProfileAsync(
            request.ExternalUserId, request.Name, request.LastName, cancellationToken);
        return result.Succeeded;
    }
}
