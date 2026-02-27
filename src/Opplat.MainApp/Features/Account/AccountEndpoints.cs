using System.Security.Claims;
using MediatR;
using Opplat.MainApp.Dtos;
using Opplat.MainApp.Features.Account.Commands;
using Opplat.MainApp.Features.Account.Queries;

namespace Opplat.MainApp.Features.Account;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        // Both tenant-prefixed and legacy (no-tenant) route groups
        var authTenant = app.MapGroup("/{__tenant__}/auth/account").WithTags("Auth");
        var authLegacy = app.MapGroup("/auth/account").WithTags("Auth");

        MapEndpoints(authTenant);
        MapEndpoints(authLegacy);
    }

    private static void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/Login",
            async (Login login, IMediator mediator) =>
            {
                var result = await mediator.Send(new LoginCommand(login.UserName, login.Password));
                if (result == null)
                    return Results.BadRequest(new { Result = false, Message = "Intento de autenticacion incorrecto." });

                return Results.Ok(new
                {
                    token      = result.Token,
                    expiration = result.Expiration,
                    userId     = result.UserId
                });
            })
            .AllowAnonymous()
            .WithSummary("Login")
            .WithDescription("Autentica un usuario y retorna un JWT");

        group.MapGet("/user-list",
            async (IMediator mediator) =>
            {
                var users = await mediator.Send(new GetUsersQuery());
                return Results.Ok(users);
            })
            .RequireAuthorization()
            .WithSummary("Listado de usuarios");

        group.MapGet("/profile/{name}",
            async (string name, IMediator mediator) =>
            {
                var user = await mediator.Send(new GetUserProfileQuery(name));
                return user is null ? Results.NotFound() : Results.Ok(user);
            })
            .RequireAuthorization()
            .WithSummary("Perfil de usuario");

        group.MapPost("/add-user",
            async (Register register, IMediator mediator) =>
            {
                var result = await mediator.Send(
                    new RegisterUserCommand(register.Name, register.LastName,
                        register.Username, register.Email, register.Password));

                return result.Success
                    ? Results.Ok(result.User)
                    : Results.BadRequest(result.Errors);
            })
            .RequireAuthorization()
            .WithSummary("Crear usuario");

        group.MapPost("/edit-user",
            async (EditUserNameDto dto, IMediator mediator) =>
            {
                var ok = await mediator.Send(new EditUserCommand(dto.Id, dto.Name, dto.LastName));
                return ok
                    ? Results.Ok()
                    : Results.BadRequest(new { Result = false, Message = "Error modificando el usuario." });
            })
            .RequireAuthorization()
            .WithSummary("Editar nombre/apellido de usuario");

        group.MapPost("/reset-password",
            async (ResetPassword resetPassword, IMediator mediator) =>
            {
                var result = await mediator.Send(
                    new ResetPasswordCommand(resetPassword.UsuarioId, resetPassword.Contraseña));

                return result.Success ? Results.Ok() : Results.BadRequest(result.Errors);
            })
            .RequireAuthorization()
            .WithSummary("Resetear contraseña (admin)");

        group.MapPost("/change-password",
            async (ChangePassword changePassword, IMediator mediator) =>
            {
                var result = await mediator.Send(
                    new ChangePasswordCommand(changePassword.UsuarioId,
                        changePassword.ContraseñaActual, changePassword.Contraseña));

                return result.Success ? Results.Ok() : Results.BadRequest(result.Errors);
            })
            .RequireAuthorization()
            .WithSummary("Cambiar contraseña (usuario)");

        group.MapGet("/cambiar-estado",
            async (string idUsuario, IMediator mediator) =>
            {
                var found = await mediator.Send(new ToggleUserActiveCommand(idUsuario));
                return found ? Results.Ok() : Results.NotFound();
            })
            .RequireAuthorization()
            .WithSummary("Activar/desactivar usuario");

        group.MapPost("/cambiar-roles",
            async (CambiarRolesDto dto, IMediator mediator) =>
            {
                var result = await mediator.Send(new ChangeRolesCommand(dto.idUsuario, dto.Roles));
                return result.Success
                    ? Results.Ok(new { Resultado = true, Mensaje = "Roles modificados correctamente." })
                    : Results.BadRequest(new { Resultado = false, Mensaje = result.ErrorMessage });
            })
            .RequireAuthorization()
            .WithSummary("Cambiar roles de usuario");
    }
}
