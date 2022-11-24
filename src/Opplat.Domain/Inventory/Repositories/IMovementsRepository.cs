using Opplat.Domain.Inventory.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Domain.Inventory.Repositories;

public interface IMovementsRepository: IRepository<ProductMovement>
{
    Task<RepositoryResponse> Create(ProductMovement entity, int movementFactor, string unit);
    Task<RepositoryResponse> CheckIfPosible(ProductMovement entity, int movementFactor, string unit);
    Task<IEnumerable<ProductMovement>> GetByStorage(Guid id);
}
