using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Services.Sales;

public interface IProductTagService : IService<ProductTag, string>
{

}

public class ProductTagService : BaseService<ProductTag, string>, IProductTagService
{
    public ProductTagService(IProductTagRepository repo, ILogger<ProductTag> logger) : base(repo, logger)
    {
    }
}
