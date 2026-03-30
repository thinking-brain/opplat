using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Entities.Sales;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Infrastructure.Persistance.Repositories;

namespace Opplat.Infrastructure.Repositories.Sales;

public class ToppingRepository(DbContext db, ILogger<IToppingRepository> logger) : BaseRepository<Topping>(db, logger), IToppingRepository
{
}