using Opplat.Domain.Inventory.Repositories;
using Opplat.Domain.Inventory.Entities;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Infrastructure.Inventory.Repositories;

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
