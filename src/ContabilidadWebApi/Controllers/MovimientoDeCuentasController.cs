using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.ViewModels;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Services;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class MovimientoDeCuentasController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public MovimientoDeCuentasController(ContabilidadDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve un Listado de Movimientos de Cuentas
        /// </summary>
        /// <returns></returns>
        // GET: api/Movimientos
        [HttpGet]
        public IEnumerable<Movimiento> GetMovimientos()
        {
            return _context.Set<Movimiento>().Include(s => s.Cuenta).Include(s => s.Asiento).ToList();
        }

        /// <summary>
        /// Devuelve un Listado de Movimientos de Cuentas En Periodos
        /// </summary>
        /// <returns></returns>
        // GET: api/MovimientosEnPeriodo
        [HttpGet("{cuenta}/{fechaInicio}/{fechaFin}")]
        public MovimientoCuentaPeriodoVM GetMovimientoCuentaPeriodo(string cuenta, DateTime fechaInicio, DateTime fechaFin)
        {
            var diaC = _context.Set<DiaContable>().SingleOrDefault(s => s.Fecha >= fechaInicio && s.Fecha < fechaFin);
            var asiento = _context.Set<Asiento>().Include(s => s.Movimientos).Where(s => s.DiaContableId == diaC.Id).ToList();
            var movimientoCuentas = new CuentasServices(_context).GetMovimientosDeCuenta(cuenta).Where(s => s.Asiento.Fecha >= fechaInicio && s.Asiento.Fecha < fechaFin);

            var movimiento = new MovimientoCuentaPeriodoVM() { Cuenta = cuenta, Importe = movimientoCuentas.Sum(s => s.Importe) };
            return movimiento;
        }



    }
}