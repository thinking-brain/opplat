using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Admin.Commands;

public record UpdateTenantUserCommand(
    string UserId,
    string Name,
    string LastName,
    string Username,
    string Email,
    bool Active) : IRequest<bool>;

public sealed class UpdateTenantUserCommandHandler : IRequestHandler<UpdateTenantUserCommand, bool>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly IGraphUserService _graphUserService;
    private readonly ILogger<UpdateTenantUserCommandHandler> _logger;

    public UpdateTenantUserCommandHandler(
        AdminTenantCatalogDbContext db,
        IGraphUserService graphUserService,
        ILogger<UpdateTenantUserCommandHandler> logger)
    {
        _db = db;
        _graphUserService = graphUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateTenantUserCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning("UpdateTenantUserCommand: invalid UserId format: {UserId}", request.UserId);
            return false;
        }

        var tenantUser = await _db.TenantUsers
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (tenantUser is null)
        {
            _logger.LogWarning("UpdateTenantUserCommand: TenantUser {UserId} not found.", userId);
            return false;
        }

        // Sync active/inactive state with identity provider when it changes
        if (tenantUser.IsActive != request.Active)
        {
            if (request.Active)
            {
                var enableResult = await _graphUserService.EnableUserAsync(tenantUser.EntraOid, cancellationToken);
                if (!enableResult.Succeeded)
                {
                    _logger.LogError("Failed to enable identity provider user {EntraOid}: {Error}",
                        tenantUser.EntraOid, enableResult.Error);
                    return false;
                }
            }
            else
            {
                var disableResult = await _graphUserService.DisableUserAsync(tenantUser.EntraOid, cancellationToken);
                if (!disableResult.Succeeded)
                {
                    _logger.LogError("Failed to disable identity provider user {EntraOid}: {Error}",
                        tenantUser.EntraOid, disableResult.Error);
                    return false;
                }
                tenantUser.DeactivatedAt = DateTime.UtcNow;
            }

            tenantUser.IsActive = request.Active;
        }

        tenantUser.ModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated tenant user {UserId} (Active={Active}).", userId, request.Active);
        return true;
    }
}
