using System.Security.Claims;
using MediatR;
using Opplat.Application.Features.Menus.Queries;

namespace Opplat.Api.Sales.Endpoints;

public static class MenusEndpoints
{
    public static void MapMenusEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/admin/menus").WithTags("Sales");

        group.MapGet("/",
            async (ClaimsPrincipal user, IMediator mediator) =>
            {
                var username = user.Identity!.Name!;
                var roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToArray();

                var result = await mediator.Send(new GetMenusQuery(username, roles));
                return Results.Ok(new { Modulos = result.Modulos, Personalizados = result.Personalizados });
            })
            .WithSummary("Obtener menus del usuario");

        group.MapGet("/FromModulo",
            async (string modulo, ClaimsPrincipal user, IMediator mediator) =>
            {
                var roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToArray();

                var menus = await mediator.Send(new GetModuleMenuQuery(modulo, roles));
                return Results.Ok(menus);
            })
            .WithSummary("Obtener menus de un modulo");
    }
}