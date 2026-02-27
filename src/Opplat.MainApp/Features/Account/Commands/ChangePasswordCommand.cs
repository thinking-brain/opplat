using MediatR;
using Microsoft.AspNetCore.Identity;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Account.Commands;

public record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<PasswordOpResult>;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, PasswordOpResult>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        UserManager<Usuario> userManager,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger      = logger;
    }

    public async Task<PasswordOpResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return new PasswordOpResult(false, "No existe el usuario solicitado");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("Se cambio la contraseña del usuario {UserName}.", user.UserName);
            return new PasswordOpResult(true, null);
        }

        return new PasswordOpResult(false, result.Errors);
    }
}
