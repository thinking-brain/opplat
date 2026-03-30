using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Repositories.Inventory;

public class InventoryRepository(DbContext db, ILogger<IInventoryRepository> logger) : BaseRepository<ProductInventory>(db, logger), IInventoryRepository
{
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
