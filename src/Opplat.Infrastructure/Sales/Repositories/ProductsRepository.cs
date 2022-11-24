using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Infrastructure.Sales.Repositories;

public class ProductsRepository : BaseRepository<ProductForSale>, IProductRepository
{
    public ProductsRepository(DbContext db, ILogger<IProductRepository> logger) : base(db, logger)
    {
    }
}
public class ProductTagRepository : BaseRepository<ProductTag>, IProductTagRepository
{
    public ProductTagRepository(DbContext db, ILogger<IProductTagRepository> logger) : base(db, logger)
    {
    }
}