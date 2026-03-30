
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Abstractions.Repositories.Inventory;

public interface IMovementsRepository: IRepository<ProductMovement>
{
    Task<RepositoryResponse> Create(ProductMovement entity, int movementFactor, string unit);
    Task<RepositoryResponse> CheckIfPosible(ProductMovement entity, int movementFactor, string unit);
    Task<IEnumerable<ProductMovement>> GetByStorage(Guid id);
}
