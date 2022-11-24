using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Sales.Services;

public interface IProductTagService : IService<ProductTag, string>
{
    
}

public class ProductTagService : BaseService<ProductTag, string> ,IProductTagService
{
    public ProductTagService(IProductTagRepository repo, ILogger<ProductTag> logger): base (repo, logger)
    {
    }
}
