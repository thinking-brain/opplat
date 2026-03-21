using MediatR;
using Microsoft.AspNetCore.Identity;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Admin.Commands;

public record UpdateTenantUserCommand(
    string UserId,
    string Name,
    string LastName,
    string Username,
    string Email,
    bool Active) : IRequest<bool>;

public sealed class UpdateTenantUserCommandHandler : IRequestHandler<UpdateTenantUserCommand, bool>
{
    private readonly UserManager<Usuario> _userManager;

    public UpdateTenantUserCommandHandler(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(UpdateTenantUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return false;

        user.Nombres = request.Name;
        user.Apellidos = request.LastName;
        user.UserName = request.Username;
        user.Email = request.Email;
        user.Activo = request.Active;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
