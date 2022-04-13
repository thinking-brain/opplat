using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Contabilidad.Domain.Dtos;

namespace Opplat.Contabilidad.Domain.Services;

public interface ICuentaService
{
    Task<IEnumerable<CuentaDto>> GetCuentas();
    Task<IEnumerable<OperacionDto>> GetMovimientosDeCuenta(string accountName, DateTime date);
    Task<bool> AgregarOperacion(Guid cuentaCreditoId, Guid cuentaDebitoId, decimal importe,
                DateTime date, string observaciones, string user);
}
