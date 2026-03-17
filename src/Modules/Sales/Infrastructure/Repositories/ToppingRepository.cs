using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Repositories;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Modules.Sales.Infrastructure.Repositories;

public class ToppingRepository : BaseRepository<Topping>, IToppingRepository
{
    public ToppingRepository(DbContext db, ILogger<IToppingRepository> logger) : base(db, logger)
    {
    }
}