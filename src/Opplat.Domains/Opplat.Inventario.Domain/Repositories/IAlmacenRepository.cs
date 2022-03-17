using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Inventario.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Inventario.Domain.Repositories;

public interface IAlmacenRepository: IRepository<Almacen>
{
    Task<RepositoryResponse> AddMovimiento(MovimientoDeProducto movimiento);
    Task<Almacen> GetPrimaryStorage();

    Task<IEnumerable<ValeDeSalida>> GetValesDeSalida(DateTime fechaInicial, DateTime fechaFinal);

    Task<IEnumerable<DetalleDeVale>> GetDetallesDeVale(Guid valeId);

    Task<IEnumerable<MovimientoDeProducto>> GetMermas(Guid almacenId);
}
