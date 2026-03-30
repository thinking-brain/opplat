using Opplat.Domain.Entities.Accounting;

namespace Opplat.Domain.Services.Accounting;

public interface IPeriodoContableService
{
    Task<IEnumerable<DiaContable>> GetPeriodosContables();
    Task<DiaContable> GetDiaContableActual();
    Task<DiaContable> FindById(Guid id);
    Task<bool> Update(DiaContable diaContable);
}
