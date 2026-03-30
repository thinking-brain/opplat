using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Features.Inventory.Inventories;

public sealed record GetInventoriesByStorageQuery(string StorageId) : IQuery<IReadOnlyList<ProductInventory>>;

public sealed class GetInventoriesByStorageQueryHandler(IInventoryRepository repository)
    : IQueryHandler<GetInventoriesByStorageQuery, IReadOnlyList<ProductInventory>>
{
    public async Task<IReadOnlyList<ProductInventory>> Handle(GetInventoriesByStorageQuery request, CancellationToken cancellationToken)
        => [.. await repository.GetByStorage(new Guid(request.StorageId))];
}

