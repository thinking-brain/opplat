using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Services.Inventory;
using Opplat.Domain.Dtos.Inventory;

namespace Opplat.Application.Features.Inventory.MovementTypes;

public sealed record ListMovementTypesQuery() : IQuery<IReadOnlyList<MovementTypeDto>>;

public sealed class ListMovementTypesQueryHandler(IMovementTypeService service)
    : IQueryHandler<ListMovementTypesQuery, IReadOnlyList<MovementTypeDto>>
{
    public Task<IReadOnlyList<MovementTypeDto>> Handle(ListMovementTypesQuery request, CancellationToken cancellationToken)
    {
        var result = service.List();
        return Task.FromResult<IReadOnlyList<MovementTypeDto>>([.. result]);
    }
}

