using Opplat.Domain.Dtos.Accounting;
using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface ICajaService
{
    Task<bool> Extraer(decimal importe, string detalle, string user);
    Task<bool> Depositar(decimal importe, string detalle, string user);
    Task<bool> SePuedeExtraer(decimal importe);
    Task<IEnumerable<OperacionDto>> GetOperaciones(DateTime date);
    Task<IEnumerable<OperacionDto>> GetVentas(DateTime date);
    Task<IEnumerable<DenominacionDeMoneda>> GetDenominaciones();
}
