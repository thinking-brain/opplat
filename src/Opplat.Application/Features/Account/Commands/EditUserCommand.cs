using Finbuckle.MultiTenant.Abstractions;
using Opplat.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Commands;

public record EditUserCommand(string Id, string Name, string LastName) : IRequest<bool>;

public class EditUserCommandHandler : IRequestHandler<EditUserCommand, bool>
{
    private readonly AdminTenantCatalogDbContext _db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor;
    private readonly ILogger<EditUserCommandHandler> _logger;

    public EditUserCommandHandler(
        AdminTenantCatalogDbContext db,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
        ILogger<EditUserCommandHandler> logger)
    {
        _db             = db;
        _tenantAccessor = tenantAccessor;
        _logger         = logger;
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

        // Display name (Name/LastName) is managed by the identity provider.
        // The user record is confirmed to exist in this tenant.
        _logger.LogInformation(
            "EditUserCommand: profile update acknowledged for user {UserId} in tenant {TenantId}. " +
            "Display name changes must be applied via the identity provider.",
            userId, tenantId);

        return true;
    }
}
