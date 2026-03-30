using MediatR;
using Opplat.Application.Utils;
using Opplat.Domain.Models;

namespace Opplat.Application.Features.Menus.Queries;

public record GetModuleMenuQuery(string Module, string[] Roles) : IRequest<List<MenuItem>>;

public class GetModuleMenuQueryHandler : IRequestHandler<GetModuleMenuQuery, List<MenuItem>>
{
    private readonly MenuLoader _menuLoader;

    public GetModuleMenuQueryHandler(MenuLoader menuLoader)
    {
        _menuLoader = menuLoader;
    }

    public async Task<List<MenuItem>> Handle(GetModuleMenuQuery request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _menuLoader.LoadModuleMenu(request.Module, request.Roles);
    }
}
