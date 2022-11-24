using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Sales.Services;

public interface IProductService : IService<ProductForSale, string>
{
    
}

public class ProductService : BaseService<ProductForSale, string> ,IProductService
{
    public ProductService(IProductRepository repo, ILogger<ProductForSale> logger): base (repo, logger)
    {
    }
}
