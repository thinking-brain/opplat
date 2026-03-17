using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Modules.Inventory.Domain.Repositories;

public interface IProductClassificationRepository: IRepository<ProductClassification>
{

}

public interface IProductGroupRepository: IRepository<ProductGroup>
{
    Task<IEnumerable<ProductGroup>> ListWithClassification();
    Task<ProductGroup> GetWithClassification(int id);
}

public interface IProductRepository: IRepository<Product>
{
    Task<IEnumerable<Product>> ListWithGroup();
    Task<Product> GetWithGroup(string id);
}
