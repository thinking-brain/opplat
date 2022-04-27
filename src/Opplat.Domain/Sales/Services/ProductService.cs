using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Sales.Services;

public interface IProductService : IService<Product>
{
    
}

public class ProductService : BaseService<Product> ,IProductService
{
    public ProductService(IProductRepository repo): base (repo)
    {
    }
}
