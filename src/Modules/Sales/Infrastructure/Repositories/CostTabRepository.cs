using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Modules.Sales.Domain.Repositories;

namespace Opplat.Modules.Sales.Infrastructure.Repositories;

public class CostTabRepository : BaseRepository<CostTab>, ICostTabRepository
{
    public CostTabRepository(DbContext db, ILogger<IRepository<CostTab>> logger) : base(db, logger)
    {
    }
}