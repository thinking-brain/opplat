using MediatR;
using Microsoft.AspNetCore.Identity;
using Opplat.MainApp.Dtos;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Account.Commands;

public record RegisterUserCommand(
    string Name,
    string LastName,
    string Username,
    string Email,
    string Password) : IRequest<RegisterUserResult>;

public record RegisterUserResult(bool Success, AccountDto? User, object? Errors);

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        UserManager<Usuario> userManager,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new Usuario
        {
            Email      = request.Email,
            UserName   = request.Username,
            Nombres    = request.Name,
            Apellidos  = request.LastName,
            Activo     = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuario {Username} creado correctamente.", request.Username);
            return new RegisterUserResult(true, new AccountDto
            {
                UserId   = user.Id,
                Name     = user.Nombres,
                LastName = user.Apellidos,
                Username = user.UserName!,
                Email    = user.Email!,
                Active   = user.Activo,
                Roles    = new List<string>()
            }, null);
        }

        return new RegisterUserResult(false, null, result);
    }
}
