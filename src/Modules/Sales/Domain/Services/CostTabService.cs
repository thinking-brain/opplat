using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Modules.Sales.Domain.Services;

public interface ICostTabService : IService<CostTab, string>
{
    
}

public class CostTabService : BaseService<CostTab, string> ,ICostTabService
{
    public CostTabService(ICostTabRepository repo, ILogger<CostTab> logger): base (repo, logger)
    {
    }
}
