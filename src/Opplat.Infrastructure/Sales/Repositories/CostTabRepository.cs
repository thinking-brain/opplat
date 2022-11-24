using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Domain.Sales.Entities;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Sales.Repositories;

namespace Opplat.Infrastructure.Sales.Repositories;

public class CostTabRepository : BaseRepository<CostTab>, ICostTabRepository
{
    public CostTabRepository(DbContext db, ILogger<IRepository<CostTab>> logger) : base(db, logger)
    {
    }
}