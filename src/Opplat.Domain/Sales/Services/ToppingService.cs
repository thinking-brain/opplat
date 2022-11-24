using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Sales.Services;

public interface IToppingService : IService<Topping, string>
{
    
}

public class ToppingService : BaseService<Topping, string> ,IToppingService
{
    public ToppingService(IToppingRepository repo, ILogger<Topping> logger): base (repo, logger)
    {
    }
}
