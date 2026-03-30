
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Abstractions.Repositories.Inventory;

public interface IInventoryRepository: IRepository<ProductInventory>
{
    Task<ProductInventory> Get(Guid storageId, Guid productId);
    
    Task<IEnumerable<ProductInventory>> GetByStorage(Guid storageId);
}
