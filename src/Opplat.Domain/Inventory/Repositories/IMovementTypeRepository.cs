using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Inventory.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Domain.Inventory.Repositories;

public interface IMovementTypeRepository: IRepository<MovementType>
{
    Task<MovementType> FindByDescription(string description);
}
