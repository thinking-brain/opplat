using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Services.Sales;

public interface IProductService : IService<ProductForSale, string>
{

}

public class ProductService(IProductRepository repo, ILogger<ProductForSale> logger) : BaseService<ProductForSale, string>(repo, logger), IProductService
{
}
