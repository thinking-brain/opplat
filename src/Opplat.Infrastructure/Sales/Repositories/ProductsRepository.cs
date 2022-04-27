using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Opplat.Infrastructure.Sales.Repositories;

public class ProductsRepository : BaseRepository<Product>, IProductRepository
{
    public ProductsRepository(DbContext db) : base(db)
    {
    }
}