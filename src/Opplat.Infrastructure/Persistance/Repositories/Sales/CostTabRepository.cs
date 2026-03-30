using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Infrastructure.Persistance.Repositories.Sales;

public class CostTabRepository(DbContext db, ILogger<IRepository<CostTab>> logger) : BaseRepository<CostTab>(db, logger), ICostTabRepository
{
}