using MediatR;
using Microsoft.AspNetCore.Identity;
using Opplat.MainApp.Auth;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Account.Commands;

public record ChangeRolesCommand(string UserId, List<string> Roles) : IRequest<ChangeRolesResult>;

public record ChangeRolesResult(bool Success, string? ErrorMessage);

public class ChangeRolesCommandHandler : IRequestHandler<ChangeRolesCommand, ChangeRolesResult>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<ChangeRolesCommandHandler> _logger;

    public ChangeRolesCommandHandler(
        UserManager<Usuario> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<ChangeRolesCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<ChangeRolesResult> Handle(ChangeRolesCommand request, CancellationToken cancellationToken)
    {
        var normalizedRoles = request.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedRoles.Count == 0)
            return new ChangeRolesResult(false, "Debe asignar al menos un rol del tenant.");

        var invalidRoles = normalizedRoles
            .Where(role => !AuthRoles.TenantAssignable.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (invalidRoles.Count > 0)
        {
            return new ChangeRolesResult(
                false,
                $"Solo se permiten roles de tenant ({string.Join(", ", AuthRoles.TenantAssignable)}). Roles inválidos: {string.Join(", ", invalidRoles)}.");
        }

        var usuario = await _userManager.FindByIdAsync(request.UserId);
        if (usuario == null)
            return new ChangeRolesResult(false, "No existe el usuario solicitado");

        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        var removeResult = await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        if (!removeResult.Succeeded)
        {
            var msg = "Ocurrieron errores modificando los roles: " +
                      string.Join(',', removeResult.Errors.Select(e => e.Description));
            return new ChangeRolesResult(false, msg);
        }

        foreach (var rol in normalizedRoles)
        {
            if (!await _roleManager.RoleExistsAsync(rol))
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(rol));
                if (!createRoleResult.Succeeded)
                {
                    var roleError = "Ocurrieron errores creando los roles: " +
                                    string.Join(',', createRoleResult.Errors.Select(error => error.Description));
                    return new ChangeRolesResult(false, roleError);
                }
            }
        }

        var addResult = await _userManager.AddToRolesAsync(usuario, normalizedRoles);
        if (!addResult.Succeeded)
        {
            var msg = "Ocurrieron errores modificando los roles: " +
                      string.Join(',', addResult.Errors.Select(e => e.Description));
            return new ChangeRolesResult(false, msg);
        }

        _logger.LogInformation("Roles del usuario {UserName} cambiados a: {Roles}.",
            usuario.UserName, string.Join(',', normalizedRoles));

        return new ChangeRolesResult(true, null);
    }
}
