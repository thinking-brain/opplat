using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Repositories.Sales;

public class SalesRepository(DbContext db, ILogger<IRepository<Sale>> logger) : BaseRepository<Sale>(db, logger), ISalesRepository
{
}
