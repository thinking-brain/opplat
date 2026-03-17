using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Modules.Inventory.Domain.Repositories;

public interface ITFARepository: IRepository<TangibleFixedAsset>
{
    Task<RepositoryResponse> AddMovement(ProductMovement movement);
    Task<RepositoryResponse> AddTFAs(IEnumerable<TangibleFixedAsset> tfas);
}
