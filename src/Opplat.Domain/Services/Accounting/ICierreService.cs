using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface ICierreService
{
    Task<IEnumerable<CierreDeCaja>> GetCierres();
    Task<bool> SePuedeCerrar();
}
