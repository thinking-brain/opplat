using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Account.Commands;

public record ChangeRolesCommand(string UserId, List<string> Roles) : IRequest<ChangeRolesResult>;

public record ChangeRolesResult(bool Success, string? ErrorMessage);

public class ChangeRolesCommandHandler : IRequestHandler<ChangeRolesCommand, ChangeRolesResult>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly OpplatDbContext _db;
    private readonly ILogger<ChangeRolesCommandHandler> _logger;

    public ChangeRolesCommandHandler(
        UserManager<Usuario> userManager,
        OpplatDbContext db,
        ILogger<ChangeRolesCommandHandler> logger)
    {
        _userManager = userManager;
        _db          = db;
        _logger      = logger;
    }

    public async Task<ChangeRolesResult> Handle(ChangeRolesCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _userManager.FindByIdAsync(request.UserId);
        if (usuario == null)
            return new ChangeRolesResult(false, "No existe el usuario solicitado");

        // Remove all current roles
        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        var removeResult  = await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        if (!removeResult.Succeeded)
        {
            var msg = "Ocurrieron errores modificando los roles: " +
                      string.Join(',', removeResult.Errors.Select(e => e.Description));
            return new ChangeRolesResult(false, msg);
        }

        // Ensure every requested role exists in the store
        foreach (var rol in request.Roles)
        {
            if (!_db.Set<IdentityRole>().Any(r => r.Name == rol))
            {
                _db.Add(new IdentityRole
                {
                    Id             = Guid.NewGuid().ToString(),
                    Name           = rol,
                    NormalizedName = rol.ToUpper()
                });
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        // Assign new roles
        var addResult = await _userManager.AddToRolesAsync(usuario, request.Roles);
        if (!addResult.Succeeded)
        {
            var msg = "Ocurrieron errores modificando los roles: " +
                      string.Join(',', addResult.Errors.Select(e => e.Description));
            return new ChangeRolesResult(false, msg);
        }

        _logger.LogInformation("Roles del usuario {UserName} cambiados a: {Roles}.",
            usuario.UserName, string.Join(',', request.Roles));

        return new ChangeRolesResult(true, null);
    }
}
