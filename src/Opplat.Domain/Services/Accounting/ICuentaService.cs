using Opplat.Domain.Dtos.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface ICuentaService
{
    Task<IEnumerable<CuentaDto>> GetCuentas();
    Task<IEnumerable<OperacionDto>> GetMovimientosDeCuenta(string accountName, DateTime date);
    Task<bool> AgregarOperacion(Guid cuentaCreditoId, Guid cuentaDebitoId, decimal importe,
                DateTime date, string observaciones, string user);
}
