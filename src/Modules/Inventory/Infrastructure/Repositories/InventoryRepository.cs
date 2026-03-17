using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Modules.Inventory.Domain.Repositories;
using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Modules.Inventory.Infrastructure.Repositories;

public class InventoryRepository : BaseRepository<ProductInventory>, IInventoryRepository
{
    public InventoryRepository(DbContext db, ILogger<IInventoryRepository> logger) : base(db, logger)
    {
    }

    public async Task<ProductInventory> Get(Guid storageId, Guid productId)
    {
        var data = await _db.Set<ProductInventory>()
            .Include(m => m.Storage)
            .Include(m => m.Product)
            .SingleOrDefaultAsync(m => m.StorageId == storageId && m.ProductId == productId);
        return data;
    }

    public async Task<IEnumerable<ProductInventory>> GetByStorage(Guid id)
    {
        var data = await _db.Set<ProductInventory>()
            .Include(m => m.Storage)
            .Include(m => m.Product)
            .Where(m => m.StorageId == id)
            .ToListAsync();
        return data;
    }
}
