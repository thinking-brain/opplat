using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Opplat.MainApp.Features.Account.Commands;
using Opplat.MainApp.Features.Admin.Commands;
using Opplat.MainApp.Features.Admin.Queries;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Admin;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var admin = app.MapGroup("/admin")
            .WithTags("Admin")
            .RequireAuthorization("AdminOnly");

        admin.MapGet("/tenants",
            async (IMediator mediator) =>
            {
                var tenants = await mediator.Send(new GetTenantsQuery());
                return Results.Ok(tenants);
            })
            .WithSummary("List tenants");

        admin.MapPost("/tenants",
            async (UpsertTenantRequest request, IMediator mediator) =>
            {
                var tenant = await mediator.Send(new CreateTenantCommand(request));
                return tenant is null
                    ? Results.BadRequest(new { Result = false, Message = "No se pudo crear el tenant." })
                    : Results.Created($"/admin/tenants/{tenant.Identifier}", tenant);
            })
            .WithSummary("Create tenant");

        admin.MapPut("/tenants/{identifier}",
            async (string identifier, UpsertTenantRequest request, IMediator mediator) =>
            {
                var tenant = await mediator.Send(new UpdateTenantCommand(identifier, request));
                return tenant is null
                    ? Results.NotFound()
                    : Results.Ok(tenant);
            })
            .WithSummary("Update tenant");

        admin.MapDelete("/tenants/{identifier}",
            async (string identifier, IMediator mediator) =>
            {
                var deactivated = await mediator.Send(new DeactivateTenantCommand(identifier));
                return deactivated ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary("Deactivate tenant");

        admin.MapGet("/users",
            async (string? tenantIdentifier, IMediator mediator) =>
            {
                var users = await mediator.Send(new GetAdminUsersQuery(tenantIdentifier));
                return Results.Ok(users);
            })
            .WithSummary("List users across tenants");

        admin.MapGet("/tenants/{tenantIdentifier}/users",
            async (string tenantIdentifier, IMediator mediator, IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var users = await mediator.Send(new GetTenantUsersQuery());
                return Results.Ok(users);
            })
            .WithSummary("List tenant users");

        admin.MapPost("/tenants/{tenantIdentifier}/users",
            async (string tenantIdentifier, AdminCreateUserRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var user = await mediator.Send(new CreateTenantUserCommand(
                    request.Name,
                    request.LastName,
                    request.Username,
                    request.Email,
                    request.Roles));

                return user is null
                    ? Results.BadRequest(new { Result = false, Message = "No se pudo crear el usuario." })
                    : Results.Created($"/admin/tenants/{tenantIdentifier}/users/{user.UserId}", user);
            })
            .WithSummary("Create tenant user");

        admin.MapPut("/tenants/{tenantIdentifier}/users/{userId}",
            async (string tenantIdentifier, string userId, AdminUpdateUserRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var updated = await mediator.Send(new UpdateTenantUserCommand(
                    userId,
                    request.Name,
                    request.LastName,
                    request.Username,
                    request.Email,
                    request.Active));

                return updated ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary("Update tenant user metadata");

        admin.MapPut("/tenants/{tenantIdentifier}/users/{userId}/roles",
            async (string tenantIdentifier, string userId, AdminSetUserRolesRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var result = await mediator.Send(new ChangeRolesCommand(userId, request.Roles));
                return result.Success
                    ? Results.NoContent()
                    : Results.BadRequest(new { Result = false, Message = result.ErrorMessage });
            })
            .WithSummary("Replace tenant user roles");

        admin.MapPut("/tenants/{tenantIdentifier}/users/{userId}/status",
            async (string tenantIdentifier, string userId, AdminSetUserActiveRequest request, IMediator mediator,
                IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor) =>
            {
                var tenantMismatch = ValidateResolvedTenant(tenantIdentifier, tenantAccessor);
                if (tenantMismatch is not null)
                    return tenantMismatch;

                var updated = await mediator.Send(new SetUserActiveStatusCommand(userId, request.Active));
                return updated ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary("Set tenant user active state");
    }

    private static IResult? ValidateResolvedTenant(
        string routeTenantIdentifier,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    {
        var resolvedTenant = tenantAccessor.MultiTenantContext?.TenantInfo?.Identifier;
        if (string.IsNullOrWhiteSpace(resolvedTenant))
        {
            return Results.BadRequest(new
            {
                Result = false,
                Message = "The X-Tenant-Identifier header must identify the tenant for this admin request."
            });
        }

        if (!string.Equals(routeTenantIdentifier, resolvedTenant, StringComparison.OrdinalIgnoreCase))
        {
            return Results.BadRequest(new
            {
                Result = false,
                Message = "The tenant route segment must match the resolved tenant header."
            });
        }

        return null;
    }
}
