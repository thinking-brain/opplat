using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Inventario.Domain.Entities;
using Opplat.Inventario.Domain.Repositories;

namespace Opplat.Inventario.Domain.Services;

public interface IAlmacenService
{
    Task<IEnumerable<ExistenciaDeProducto>> Existencias();
    Task<IEnumerable<ValeDeSalida>> ValesDeSalida(DateTime date);
    Task<IEnumerable<ValeDeSalida>> ValesDeSalida(DateTime startDate, DateTime endDate);
    Task<bool> DarSalidaDeAlmacen(ValeDeSalida vale, string user);
    // TODO: Cambiar la clase que se pasa para la merma.
    Task<bool> DarSalidaPorMerma(ValeDeSalida vale, string user);
}

public class AlmacenService : IAlmacenService
{

}
