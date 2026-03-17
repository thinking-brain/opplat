using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Modules.Sales.Domain.Services;

public interface IProductTagService : IService<ProductTag, string>
{
    
}

public class ProductTagService : BaseService<ProductTag, string> ,IProductTagService
{
    public ProductTagService(IProductTagRepository repo, ILogger<ProductTag> logger): base (repo, logger)
    {
    }
}
