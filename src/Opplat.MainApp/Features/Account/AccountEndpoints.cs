using MediatR;
using Opplat.MainApp.Dtos;
using Opplat.MainApp.Features.Account.Queries;
using Opplat.MainApp.Features.Account.Commands;

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
            () =>
            {
                return Results.BadRequest(new
                {
                    Result = false,
                    Message = "Interactive login is handled by the configured identity provider. Obtain an access token from Auth0 or Keycloak and send it as a bearer token."
                });
            })
            .AllowAnonymous()
            .WithSummary("OIDC login handoff")
            .WithDescription("The backend no longer issues local JWTs; use the external identity provider login flow.");

        group.MapGet("/user-list",
            async (IMediator mediator) =>
            {
                var users = await mediator.Send(new GetUsersQuery());
                return Results.Ok(users);
            })
            .RequireAuthorization("TenantAdminOnly")
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
            .RequireAuthorization("TenantAdminOnly")
            .WithSummary("Crear usuario");

        group.MapPost("/edit-user",
            async (EditUserNameDto dto, IMediator mediator) =>
            {
                var ok = await mediator.Send(new EditUserCommand(dto.Id, dto.Name, dto.LastName));
                return ok
                    ? Results.Ok()
                    : Results.BadRequest(new { Result = false, Message = "Error modificando el usuario." });
            })
            .RequireAuthorization("TenantAdminOnly")
            .WithSummary("Editar nombre/apellido de usuario");

        group.MapPost("/reset-password",
            () =>
            {
                return Results.BadRequest(new
                {
                    Result = false,
                    Message = "Password reset is managed by the configured identity provider."
                });
            })
            .RequireAuthorization()
            .WithSummary("Password reset delegated to IdP");

        group.MapPost("/change-password",
            () =>
            {
                return Results.BadRequest(new
                {
                    Result = false,
                    Message = "Password changes are managed by the configured identity provider."
                });
            })
            .RequireAuthorization()
            .WithSummary("Password change delegated to IdP");

        group.MapGet("/cambiar-estado",
            async (string idUsuario, IMediator mediator) =>
            {
                var found = await mediator.Send(new ToggleUserActiveCommand(idUsuario));
                return found ? Results.Ok() : Results.NotFound();
            })
            .RequireAuthorization("TenantAdminOnly")
            .WithSummary("Activar/desactivar usuario");

        group.MapPost("/cambiar-roles",
            async (CambiarRolesDto dto, IMediator mediator) =>
            {
                var result = await mediator.Send(new ChangeRolesCommand(dto.idUsuario, dto.Roles));
                return result.Success
                    ? Results.Ok(new { Resultado = true, Mensaje = "Roles modificados correctamente." })
                    : Results.BadRequest(new { Resultado = false, Mensaje = result.ErrorMessage });
            })
            .RequireAuthorization("TenantAdminOnly")
            .WithSummary("Cambiar roles de usuario");
    }
}
