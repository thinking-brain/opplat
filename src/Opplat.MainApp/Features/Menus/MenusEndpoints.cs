using System.Security.Claims;
using MediatR;
using Opplat.MainApp.Features.Menus.Queries;

namespace Opplat.MainApp.Features.Menus;

public static class MenusEndpoints
{
    public static void MapMenusEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/admin/menus").WithTags("Menus");

        group.MapGet("/",
            async (ClaimsPrincipal user, IMediator mediator) =>
            {
                var username = user.Identity!.Name!;
                var roles    = user.Claims
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
