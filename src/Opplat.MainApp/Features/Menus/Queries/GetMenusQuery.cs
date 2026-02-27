using System.Security.Claims;
using MediatR;
using Opplat.MainApp.Models;
using Opplat.MainApp.Utils;

namespace Opplat.MainApp.Features.Menus.Queries;

public record GetMenusQuery(string Username, string[] Roles) : IRequest<GetMenusResult>;

public record GetMenusResult(List<MenuItem> Modulos, List<MenuItem> Personalizados);

public class GetMenusQueryHandler : IRequestHandler<GetMenusQuery, GetMenusResult>
{
    private readonly MenuLoader _menuLoader;

    public GetMenusQueryHandler(MenuLoader menuLoader)
    {
        _menuLoader = menuLoader;
    }

    public async Task<GetMenusResult> Handle(GetMenusQuery request, CancellationToken cancellationToken)
    {
        var personales = _menuLoader.LoadPersonalizeMenu(request.Username);
        var modulos    = _menuLoader.LoadAllModulesMenu(request.Roles);

        await Task.CompletedTask;
        return new GetMenusResult(modulos, personales);
    }
}
