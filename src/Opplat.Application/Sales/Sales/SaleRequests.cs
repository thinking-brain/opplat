using Opplat.Application.Abstractions.Messaging;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Repositories;

namespace Opplat.Application.Sales.Sales;

public sealed record ListSalesQuery() : IQuery<IReadOnlyList<Sale>>;

public sealed class ListSalesQueryHandler(ISalesRepository repository)
    : IQueryHandler<ListSalesQuery, IReadOnlyList<Sale>>
{
    public async Task<IReadOnlyList<Sale>> Handle(ListSalesQuery request, CancellationToken cancellationToken)
        => (await repository.List()).ToList();
}

