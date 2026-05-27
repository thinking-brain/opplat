using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Commands;

public record ResetPasswordCommand(string UserId, string NewPassword) : IRequest<PasswordOpResult>;

public record PasswordOpResult(bool Success, object? Errors);

public class ResetPasswordCommandHandler(
    AdminTenantCatalogDbContext db,
    IUserManagementService userManagementService,
    ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand, PasswordOpResult>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IUserManagementService _userManagementService = userManagementService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger = logger;

    public async Task<PasswordOpResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return new PasswordOpResult(false, "Invalid user ID format.");

        var tenantUser = await _db.TenantUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (tenantUser is null)
            return new PasswordOpResult(false, "User not found.");

        if (string.IsNullOrWhiteSpace(tenantUser.EntraOid))
            return new PasswordOpResult(false, "User has no identity provider account linked.");

        var result = await _userManagementService.ResetPasswordAsync(
            tenantUser.EntraOid, request.NewPassword, cancellationToken);

        if (result.Succeeded)
        {
            _logger.LogInformation(
                "Password reset for user {UserId} (OID: {Oid}).",
                userId, tenantUser.EntraOid);
            return new PasswordOpResult(true, null);
        }

        _logger.LogWarning(
            "Password reset failed for user {UserId}: {Error}.",
            userId, result.Error);
        return new PasswordOpResult(false, result.Error);
    }
}
