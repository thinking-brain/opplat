using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Repositories.Sales;

public class ProductsRepository(DbContext db, ILogger<IProductRepository> logger) : BaseRepository<ProductForSale>(db, logger), IProductRepository
{
}

public class ProductTagRepository(DbContext db, ILogger<IProductTagRepository> logger) : BaseRepository<ProductTag>(db, logger), IProductTagRepository
{
}