using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Domain.Sales.Entities;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Infrastructure.Sales.Repositories;

public class SalesRepository : BaseRepository<Sale>
{
    public SalesRepository(DbContext db, ILogger<IRepository<Sale>> logger) : base(db, logger)
    {
    }
}