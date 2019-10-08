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
    public class MovimientoDeCuentasAcumuladoController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public MovimientoDeCuentasAcumuladoController(ContabilidadDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve un Listado de Movimientos de Cuentas Acumulado En Periodos 
        /// </summary>
        /// <returns></returns>
        // GET: api/MovimientosEnPeriodoAcumulado
        [HttpGet("{cuenta}/{fechaInicio}/{fechaFin}")]
        public MovimientoCuentaPeriodoVM GetMovimientoCuentaPeriodoAcumulado(string cuenta, DateTime fechaInicio, DateTime fechaFin)
        {
            var mov = new List<MovimientoCuentaPeriodoVM>();
            var diaC = _context.Set<DiaContable>().Where(s => s.Fecha >= fechaInicio && s.Fecha < fechaFin);
            foreach (var dia in diaC)
            {
                var asiento = _context.Set<Asiento>().Include(s => s.Movimientos).Where(s => s.DiaContableId == dia.Id).ToList();
                var movimientoCuentas = new CuentasServices(_context).GetMovimientosDeCuenta(cuenta).Where(s => s.Asiento.Fecha >= fechaInicio && s.Asiento.Fecha < fechaFin);
                var movimiento = new MovimientoCuentaPeriodoVM() { Cuenta = cuenta, Importe = movimientoCuentas.Sum(s => s.Importe) };
                mov.Add(movimiento);
            }
            var movimientoAcum = new MovimientoCuentaPeriodoVM() { Cuenta = cuenta, Importe = mov.Sum(s => s.Importe) };


            return movimientoAcum;
        }

    }
}