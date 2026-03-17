using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Modules.Inventory.Domain.Repositories;

public interface IMovementsRepository: IRepository<ProductMovement>
{
    Task<RepositoryResponse> Create(ProductMovement entity, int movementFactor, string unit);
    Task<RepositoryResponse> CheckIfPosible(ProductMovement entity, int movementFactor, string unit);
    Task<IEnumerable<ProductMovement>> GetByStorage(Guid id);
}
