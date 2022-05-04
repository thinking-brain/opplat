using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Repositories;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Opplat.Infrastructure.Sales.Repositories;

public class ToppingRepository : BaseRepository<Topping>, IToppingRepository
{
    public ToppingRepository(DbContext db) : base(db)
    {
    }
}