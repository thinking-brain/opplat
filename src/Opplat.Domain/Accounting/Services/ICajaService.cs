using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Contabilidad.Domain.Dtos;
using Opplat.Contabilidad.Domain.Entities;

namespace Opplat.Contabilidad.Domain.Services;

public interface ICajaService
{
    Task<bool> Extraer(decimal importe, string detalle, string user);
    Task<bool> Depositar(decimal importe, string detalle, string user);
    Task<bool> SePuedeExtraer(decimal importe);
    Task<IEnumerable<OperacionDto>> GetOperaciones(DateTime date);
    Task<IEnumerable<OperacionDto>> GetVentas(DateTime date);
    Task<IEnumerable<DenominacionDeMoneda>> GetDenominaciones();
}
