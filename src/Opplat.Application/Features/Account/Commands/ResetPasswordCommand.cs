using MediatR;
using Microsoft.Extensions.Logging;

namespace Opplat.Application.Features.Account.Commands;

public record ResetPasswordCommand(string UserId, string NewPassword) : IRequest<PasswordOpResult>;

public record PasswordOpResult(bool Success, object? Errors);

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, PasswordOpResult>
{
    // private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        // UserManager<Usuario> userManager,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        // _userManager = userManager;
        _logger      = logger;
    }

    public async Task<PasswordOpResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // var user = await _userManager.FindByIdAsync(request.UserId);
        // if (user == null) return new PasswordOpResult(false, "No existe el usuario solicitado");

        // await _userManager.RemovePasswordAsync(user);
        // var result = await _userManager.AddPasswordAsync(user, request.NewPassword);

        // if (result.Succeeded)
        // {
        //     _logger.LogInformation("Se reseteo la contraseña del usuario {UserName}.", user.UserName);
        //     return new PasswordOpResult(true, null);
        // }

        // return new PasswordOpResult(false, result.Errors);
        return new PasswordOpResult(false, null);
    }
}
