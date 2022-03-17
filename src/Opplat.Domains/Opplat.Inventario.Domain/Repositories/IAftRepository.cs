using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Inventario.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Inventario.Domain.Repositories;

public interface IAftRepository: IRepository<Aft>
{
    Task<RepositoryResponse> AddMovimiento(MovimientoDeProducto movimiento);
    Task<RepositoryResponse> AddAfts(IEnumerable<Aft> afts);
}
