using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Sales.Services;

public interface IToppingService : IService<Topping>
{
    
}

public class ToppingService : BaseService<Topping> ,IToppingService
{
    public ToppingService(IToppingRepository repo): base (repo)
    {
    }
}
