using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Repositories;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Infrastructure.Inventory.Repositories;

public class ProductClassificationRepository : BaseRepository<ProductClassification>, IProductClassificationRepository
{
    public ProductClassificationRepository(DbContext db, ILogger<IProductClassificationRepository> logger) : base(db, logger)
    {
    }
}

public class ProductGroupRepository : BaseRepository<ProductGroup>, IProductGroupRepository
{
    public ProductGroupRepository(DbContext db, ILogger<IProductGroupRepository> logger) : base(db, logger)
    {
    }

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

public class ProductsRepository : BaseRepository<Product>, IProductRepository
{
    public ProductsRepository(DbContext db, ILogger<IProductRepository> logger) : base(db, logger)
    {
    }

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