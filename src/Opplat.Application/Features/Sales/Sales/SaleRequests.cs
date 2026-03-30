using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Features.Sales.Sales;

public sealed record ListSalesQuery() : IQuery<IReadOnlyList<Sale>>;

public sealed class ListSalesQueryHandler(ISalesRepository repository)
    : IQueryHandler<ListSalesQuery, IReadOnlyList<Sale>>
{
    public async Task<IReadOnlyList<Sale>> Handle(ListSalesQuery request, CancellationToken cancellationToken)
        => [.. await repository.List()];
}

