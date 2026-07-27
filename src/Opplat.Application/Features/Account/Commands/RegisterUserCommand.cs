using Finbuckle.MultiTenant.Abstractions;
using Opplat.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Identity;
using Opplat.Application.Dtos;
using Opplat.Domain.Entities.Administration;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Application.Features.Account.Commands;

public record RegisterUserCommand(
    string Name,
    string LastName,
    string Username,
    string Email,
    string Password) : IRequest<RegisterUserResult>;

public record RegisterUserResult(bool Success, AccountDto? User, object? Errors);

public class RegisterUserCommandHandler(
    AdminTenantCatalogDbContext db,
    IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
    IUserManagementService userManagementService,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly AdminTenantCatalogDbContext _db = db;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor = tenantAccessor;
    private readonly IUserManagementService _userManagementService = userManagementService;
    private readonly ILogger<RegisterUserCommandHandler> _logger = logger;

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Resolve tenant context
        var tenantInfo = _tenantAccessor.MultiTenantContext?.TenantInfo;
        if (tenantInfo is null || !Guid.TryParse(tenantInfo.Id, out var tenantId))
        {
            _logger.LogWarning("RegisterUserCommand: no valid tenant context for user {Email}.", request.Email);
            return new RegisterUserResult(false, null, "No tenant context available.");
        }

        // 2. Look up tenant with subscription plan and active user count
        var tenant = await _db.Tenants
            .Include(t => t.SubscriptionPlan)
            .Include(t => t.TenantUsers)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant is null)
        {
            _logger.LogWarning("RegisterUserCommand: tenant {TenantId} not found.", tenantId);
            return new RegisterUserResult(false, null, "Tenant not found.");
        }

        // 3. Check seat limit
        if (tenant.SubscriptionPlan is not null)
        {
            var activeCount = tenant.TenantUsers.Count(u => u.IsActive);
            if (activeCount >= tenant.SubscriptionPlan.MaxActiveUsers)
            {
                _logger.LogWarning(
                    "Tenant {TenantIdentifier} has reached the maximum of {Max} active users.",
                    tenant.Identifier, tenant.SubscriptionPlan.MaxActiveUsers);
                return new RegisterUserResult(false, null, "User seat limit reached for this tenant.");
            }
        }

        // 4. Check for duplicate email within tenant
        var duplicate = await _db.TenantUsers
            .AnyAsync(u => u.TenantId == tenantId && u.Email == request.Email, cancellationToken);
        if (duplicate)
        {
            _logger.LogWarning(
                "RegisterUserCommand: email {Email} already registered in tenant {TenantId}.",
                request.Email, tenantId);
            return new RegisterUserResult(false, null, "A user with this email already exists in this tenant.");
        }

        // 5. Create user in identity provider
        var temporaryPassword = string.IsNullOrWhiteSpace(request.Password)
            ? $"Tmp!{Guid.NewGuid():N}1A"
            : request.Password;

        var idpResult = await _userManagementService.CreateUserAsync(new CreateUserRequest
        {
            Email = request.Email,
            UserName = $"{request.Username}.{request.LastName}@{tenantInfo.Identifier}".ToLowerInvariant().Trim(),
            FirstName = request.Name,
            LastName = request.LastName,
            Password = temporaryPassword
        }, cancellationToken);

        if (!idpResult.Succeeded)
        {
            _logger.LogError(
                "RegisterUserCommand: IdP user creation failed for {Email}: {Error}.",
                request.Email, idpResult.Error);
            return new RegisterUserResult(false, null, idpResult.Error);
        }

        // 6. Create TenantUser record in catalog
        var tenantUser = new TenantUser
        {
            Id = Guid.NewGuid(),
            EntraOid = idpResult.ObjectId ?? string.Empty,
            TenantId = tenantId,
            Email = request.Email,
            Role = TenantUserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
        _db.TenantUsers.Add(tenantUser);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Registered user {Email} (OID: {Oid}) for tenant {TenantIdentifier}.",
            request.Email, idpResult.ObjectId, tenant.Identifier);

        return new RegisterUserResult(true, new AccountDto
        {
            UserId = tenantUser.Id,
            Name = request.Name,
            LastName = request.LastName,
            Username = request.Email,
            Email = request.Email,
            Active = true,
            Roles = [AuthRoles.TenantUser]
        }, null);
    }
}
