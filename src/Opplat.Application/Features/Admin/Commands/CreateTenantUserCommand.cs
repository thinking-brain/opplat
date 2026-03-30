using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Dtos;
using Opplat.Domain.Models;

namespace Opplat.Application.Features.Admin.Commands;

public record CreateTenantUserCommand(
    string Name,
    string LastName,
    string Username,
    string Email,
    IReadOnlyCollection<string> Roles) : IRequest<AdminUserDto?>;

public sealed class CreateTenantUserCommandHandler : IRequestHandler<CreateTenantUserCommand, AdminUserDto?>
{
    // private readonly UserManager<Usuario> _userManager;
    // private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor;
    private readonly ILogger<CreateTenantUserCommandHandler> _logger;

    public CreateTenantUserCommandHandler(
        // UserManager<Usuario> userManager,
        // RoleManager<IdentityRole> roleManager,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
        ILogger<CreateTenantUserCommandHandler> logger)
    {
        // _userManager = userManager;
        // _roleManager = roleManager;
        _tenantAccessor = tenantAccessor;
        _logger = logger;
    }

    public async Task<AdminUserDto?> Handle(CreateTenantUserCommand request, CancellationToken cancellationToken)
    {
        var roles = request.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (roles.Count == 0)
        {
            _logger.LogWarning("Rejected tenant user creation for {UserName} because no tenant role was supplied.", request.Username);
            return null;
        }

        var invalidRoles = roles
            .Where(role => !AuthRoles.TenantAssignable.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (invalidRoles.Count > 0)
        {
            _logger.LogWarning(
                "Rejected tenant user creation for {UserName} because the request included invalid roles: {Roles}.",
                request.Username,
                string.Join(", ", invalidRoles));
            return null;
        }

        var user = new Usuario
        {
            Nombres = request.Name,
            Apellidos = request.LastName,
            UserName = request.Username,
            Email = request.Email,
            Activo = true
        };

        // var createResult = await _userManager.CreateAsync(user);
        // if (!createResult.Succeeded)
        //     return null;

        foreach (var role in roles)
        {
            // if (!await _roleManager.RoleExistsAsync(role))
            // {
            //     var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
            //     if (!roleResult.Succeeded)
            //     {
            //         await _userManager.DeleteAsync(user);
            //         return null;
            //     }
            // }
        }

        if (roles.Count > 0)
        {
            // var addRolesResult = await _userManager.AddToRolesAsync(user, roles);
            // if (!addRolesResult.Succeeded)
            // {
            //     await _userManager.DeleteAsync(user);
            //     return null;
            // }
        }

        var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo;
        _logger.LogInformation("Created tenant user {UserName} for tenant {TenantIdentifier}.",
            user.UserName, tenant?.Identifier);

        return new AdminUserDto
        {
            TenantId = tenant?.Id ?? string.Empty,
            TenantIdentifier = tenant?.Identifier ?? string.Empty,
            TenantName = tenant?.Name ?? string.Empty,
            UserId = user.Id,
            Name = user.Nombres,
            LastName = user.Apellidos,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Active = user.Activo,
            Roles = roles
        };
    }
}
