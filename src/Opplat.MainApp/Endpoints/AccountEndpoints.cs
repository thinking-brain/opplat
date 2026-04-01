using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Account.Commands;
using Opplat.Application.Features.Account.Queries;
using Opplat.Application.Services;
using Opplat.Domain.Models;
using Opplat.MainApp.Features.Account.Queries;

namespace Opplat.MainApp.Endpoints;

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

        group.MapGet("/tenant-context",
            async (HttpContext httpContext,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor,
                [FromServices] TenantCatalogStore tenantStore) =>
            {
                var tenantInfo = tenantAccessor.MultiTenantContext?.TenantInfo;
                var claimTenantIdentifier = httpContext.User.FindFirst(AuthClaimTypes.TenantIdentifier)?.Value;
                var claimTenantId = httpContext.User.FindFirst(AuthClaimTypes.TenantId)?.Value;

                if (tenantInfo is null && !string.IsNullOrWhiteSpace(claimTenantIdentifier))
                    tenantInfo = await tenantStore.TryGetByIdentifierAsync(claimTenantIdentifier);

                if (tenantInfo is null && !string.IsNullOrWhiteSpace(claimTenantId))
                    tenantInfo = await tenantStore.TryGetAsync(claimTenantId);

                if (tenantInfo is null)
                {
                    return Results.Ok(new TenantAccessContextDto
                    {
                        IsResolved = false,
                        IsActive = false,
                        Status = "unresolved",
                        TenantId = claimTenantId,
                        TenantIdentifier = claimTenantIdentifier,
                        Message = "No tenant could be resolved for the current user."
                    });
                }

                return Results.Ok(new TenantAccessContextDto
                {
                    IsResolved = true,
                    IsActive = tenantInfo.IsActive,
                    Status = tenantInfo.IsActive ? "active" : "inactive",
                    TenantId = tenantInfo.Id,
                    TenantIdentifier = tenantInfo.Identifier,
                    TenantName = tenantInfo.Name,
                    Message = tenantInfo.IsActive
                        ? "Tenant access granted."
                        : "Tenant is inactive."
                });
            })
            .RequireAuthorization()
            .WithSummary("Contexto de tenant resuelto");

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
            async (string userId, IMediator mediator) =>
            {
                var found = await mediator.Send(new ToggleUserActiveCommand(userId));
                return found ? Results.Ok() : Results.NotFound();
            })
            .RequireAuthorization("TenantAdminOnly")
            .WithSummary("Activar/desactivar usuario");

        group.MapPost("/cambiar-roles",
            async (ChangeRolesDto dto, IMediator mediator) =>
            {
                var result = await mediator.Send(new ChangeRolesCommand(dto.UserId, dto.Roles));
                return result.Success
                    ? Results.Ok(new { Resultado = true, Mensaje = "Roles modificados correctamente." })
                    : Results.BadRequest(new { Resultado = false, Mensaje = result.ErrorMessage });
            })
            .RequireAuthorization("TenantAdminOnly")
            .WithSummary("Cambiar roles de usuario");
    }
}
