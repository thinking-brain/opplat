using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Contabilidad.Domain.Dtos;
using Opplat.Contabilidad.Domain.Entities;

namespace Opplat.Contabilidad.Domain.Services;

public interface IPeriodoContableService
{
    Task<IEnumerable<DiaContable>> GetPeriodosContables();
    Task<DiaContable> GetDiaContableActual();
    Task<DiaContable> FindById(Guid id);
    Task<bool> Update(DiaContable diaContable);
}
