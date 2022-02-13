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
    public class MovimientoDeCuentasAcumuladoController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;
        private readonly CuentasServices _cuentaService;
        private readonly CuentasHelper _cuentaHelper;

        public MovimientoDeCuentasAcumuladoController(ContabilidadDbContext context, CuentasServices cuentaService)
        {
            _context = context;
            _cuentaService = cuentaService;
            _cuentaHelper = new CuentasHelper(context);
        }

        /// <summary>
        /// Devuelve un Listado de Movimientos de Cuentas Acumulado En Periodos 
        /// </summary>
        /// <returns></returns>
        // GET: api/MovimientosEnPeriodoAcumulado
        [HttpGet("{cuenta}/{fechaInicio}/{fechaFin}")]
        public MovimientoCuentaPeriodoVM GetMovimientoCuentaPeriodoAcumulado(string cuenta, DateTime fechaInicio, DateTime fechaFin)
        {
            var cuentaContable = _cuentaService.FindCuentaByNumero(cuenta);
            var movimientoCuentas = _cuentaService.MovimientosDeCuentaYDescendientes(cuentaContable.Id, fechaInicio, fechaFin);
            var movimientoAcum = new MovimientoCuentaPeriodoVM() { Cuenta = cuenta, Importe = movimientoCuentas.Sum(s => _cuentaHelper.ImporteMovimiento(s.Cuenta.Naturaleza, s.TipoDeOperacion, s.Importe)) };
            return movimientoAcum;
        }

    }
}