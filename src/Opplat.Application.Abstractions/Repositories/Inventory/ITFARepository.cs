using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Abstractions.Repositories.Inventory;

public interface ITFARepository : IRepository<TangibleFixedAsset>
{
    Task<RepositoryResponse> AddMovement(ProductMovement movement);
    Task<RepositoryResponse> AddTFAs(IEnumerable<TangibleFixedAsset> tfas);
}
