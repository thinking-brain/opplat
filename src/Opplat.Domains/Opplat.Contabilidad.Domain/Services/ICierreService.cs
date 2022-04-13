using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Contabilidad.Domain.Dtos;
using Opplat.Contabilidad.Domain.Entities;

namespace Opplat.Contabilidad.Domain.Services;

public interface ICierreService
{
    Task<IEnumerable<CierreDeCaja>> GetCierres();
    Task<bool> SePuedeCerrar();
}
