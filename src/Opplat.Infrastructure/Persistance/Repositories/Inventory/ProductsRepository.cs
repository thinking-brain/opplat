using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Repositories.Inventory;

public class ProductClassificationRepository(DbContext db, ILogger<IProductClassificationRepository> logger) : BaseRepository<ProductClassification>(db, logger), IProductClassificationRepository
{
}

public class ProductGroupRepository(DbContext db, ILogger<IProductGroupRepository> logger) : BaseRepository<ProductGroup>(db, logger), IProductGroupRepository
{
    public async Task<ProductGroup> GetWithClassification(int id)
    {
        var data = await base.Query().Include(g => g.Classification).FirstOrDefaultAsync(g => g.Id == id);
        return data;
    }

    public async Task<IEnumerable<ProductGroup>> ListWithClassification()
    {
        var data = await base.Query().Include(g => g.Classification).ToListAsync();
        return data;
    }
}

public class ProductsRepository(DbContext db, ILogger<IProductRepository> logger) : BaseRepository<Product>(db, logger), IProductRepository
{
    public async Task<Product> GetWithGroup(string id)
    {
        var idGuid = new Guid(id);
        var data = await base.Query().Include(g => g.Group).FirstOrDefaultAsync(g => g.Id == idGuid);
        return data;
    }

    public async Task<IEnumerable<Product>> ListWithGroup()
    {
        var data = await base.Query().Include(g => g.Group).ToListAsync();
        return data;
    }
}