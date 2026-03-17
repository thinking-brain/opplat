using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Modules.Inventory.Domain.Repositories;

public interface IInventoryRepository: IRepository<ProductInventory>
{
    Task<ProductInventory> Get(Guid storageId, Guid productId);
    
    Task<IEnumerable<ProductInventory>> GetByStorage(Guid storageId);
}
