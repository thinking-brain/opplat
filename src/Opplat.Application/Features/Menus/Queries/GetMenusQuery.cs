using MediatR;
using Opplat.Application.Utils;
using Opplat.Domain.Models;

namespace Opplat.Application.Features.Menus.Queries;

public record GetMenusQuery(string Username, string[] Roles) : IRequest<GetMenusResult>;

public record GetMenusResult(List<MenuItem> Modulos, List<MenuItem> Personalizados);

public class GetMenusQueryHandler(MenuLoader menuLoader) : IRequestHandler<GetMenusQuery, GetMenusResult>
{
    private readonly MenuLoader _menuLoader = menuLoader;

    public async Task<GetMenusResult> Handle(GetMenusQuery request, CancellationToken cancellationToken)
    {
        var personales = _menuLoader.LoadPersonalizeMenu(request.Username);
        var modulos    = _menuLoader.LoadAllModulesMenu(request.Roles);

        await Task.CompletedTask;
        return new GetMenusResult(modulos, personales);
    }
}
