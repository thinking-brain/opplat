using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Modules.Sales.Infrastructure.Repositories;

public class SalesRepository : BaseRepository<Sale>
{
    public SalesRepository(DbContext db, ILogger<IRepository<Sale>> logger) : base(db, logger)
    {
    }
}