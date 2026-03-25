using Opplat.Application.Abstractions.Messaging;
using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Modules.Inventory.Domain.Repositories;

namespace Opplat.Application.Inventory.Inventories;

public sealed record GetInventoriesByStorageQuery(string StorageId) : IQuery<IReadOnlyList<ProductInventory>>;

public sealed class GetInventoriesByStorageQueryHandler(IInventoryRepository repository)
    : IQueryHandler<GetInventoriesByStorageQuery, IReadOnlyList<ProductInventory>>
{
    public async Task<IReadOnlyList<ProductInventory>> Handle(GetInventoriesByStorageQuery request, CancellationToken cancellationToken)
        => (await repository.GetByStorage(new Guid(request.StorageId))).ToList();
}

