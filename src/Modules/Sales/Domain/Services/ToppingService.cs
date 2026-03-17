using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Modules.Sales.Domain.Services;

public interface IToppingService : IService<Topping, string>
{
    
}

public class ToppingService : BaseService<Topping, string> ,IToppingService
{
    public ToppingService(IToppingRepository repo, ILogger<Topping> logger): base (repo, logger)
    {
    }
}
