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
using ContabilidadWebApi.Helpers;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class MovimientoDeCuentasController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;
        private readonly CuentasServices _cuentaService;
        private readonly CuentasHelper _cuentaHelper;

        public MovimientoDeCuentasController(ContabilidadDbContext context, CuentasServices cuentaService)
        {
            _context = context;
            _cuentaService = cuentaService;
            _cuentaHelper = new CuentasHelper(context);
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

            var cuentaContable = _cuentaService.FindCuentaByNumero(cuenta);
            if (cuentaContable != null)
            {
                var movimientoCuentas = _cuentaService.MovimientosDeCuentaYDescendientes(cuentaContable.Id, fechaInicio, fechaFin);
                var movimiento = new MovimientoCuentaPeriodoVM() { Cuenta = cuenta, Importe = movimientoCuentas.Sum(s => _cuentaHelper.ImporteMovimiento(s.Cuenta.Naturaleza, s.TipoDeOperacion, s.Importe)) };
                return movimiento;
            }
            var movimientoVacio = new MovimientoCuentaPeriodoVM() { Cuenta = cuenta, Importe = 0 };
            return movimientoVacio;
        }



    }
}