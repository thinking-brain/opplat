using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Sales.Services;

public interface ICostTabService : IService<CostTab, string>
{
    
}

public class CostTabService : BaseService<CostTab, string> ,ICostTabService
{
    public CostTabService(ICostTabRepository repo, ILogger<CostTab> logger): base (repo, logger)
    {
    }
}
