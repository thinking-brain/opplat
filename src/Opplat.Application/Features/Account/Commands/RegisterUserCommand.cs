using MediatR;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Dtos;
using Opplat.Domain.Models;

namespace Opplat.Application.Features.Account.Commands;

public record RegisterUserCommand(
    string Name,
    string LastName,
    string Username,
    string Email,
    string Password) : IRequest<RegisterUserResult>;

public record RegisterUserResult(bool Success, AccountDto? User, object? Errors);

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    // private readonly UserManager<Usuario> _userManager;
    // private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        // UserManager<Usuario> userManager,
        // RoleManager<IdentityRole> roleManager,
        ILogger<RegisterUserCommandHandler> logger)
    {
        // _userManager = userManager;
        // _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = request.Username,
            Name = request.Name,
            LastName = request.LastName,
            IsActive = true
        };

        // var result = await _userManager.CreateAsync(user);

        // if (result.Succeeded)
        // {
        //     if (!await _roleManager.RoleExistsAsync(AuthRoles.TenantUser))
        //     {
        //         var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(AuthRoles.TenantUser));
        //         if (!createRoleResult.Succeeded)
        //         {
        //             await _userManager.DeleteAsync(user);
        //             return new RegisterUserResult(false, null, createRoleResult.Errors);
        //         }
        //     }

        //     var addRoleResult = await _userManager.AddToRoleAsync(user, AuthRoles.TenantUser);
        //     if (!addRoleResult.Succeeded)
        //     {
        //         await _userManager.DeleteAsync(user);
        //         return new RegisterUserResult(false, null, addRoleResult.Errors);
        //     }

        //     _logger.LogInformation(
        //         "Usuario {Username} creado correctamente sin contraseña local; el IdP administra las credenciales y se asignó el rol {Role}.",
        //         request.Username,
        //         AuthRoles.TenantUser);

        //     return new RegisterUserResult(true, new AccountDto
        //     {
        //         UserId = user.Id,
        //         Name = user.Nombres,
        //         LastName = user.Apellidos,
        //         Username = user.UserName!,
        //         Email = user.Email!,
        //         Active = user.Activo,
        //         Roles = [AuthRoles.TenantUser]
        //     }, null);
        // }

        // return new RegisterUserResult(false, null, result);
        return new RegisterUserResult(false, null, null);
    }
}
