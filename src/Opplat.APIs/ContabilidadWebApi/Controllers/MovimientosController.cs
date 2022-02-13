using System;
using System.Linq;
using System.Threading.Tasks;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Dtos;
using ContabilidadWebApi.Helpers;
using ContabilidadWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;
        private readonly CuentasServices _cuentaService;
        private readonly CuentasHelper _cuentaHelper;

        public MovimientosController(ContabilidadDbContext context, CuentasServices cuentaService)
        {
            _context = context;
            _cuentaService = cuentaService;
            _cuentaHelper = new CuentasHelper(context);
        }

        /// <summary>
        /// Devuelve listado de operaciones de una cuenta en un periodo de tiempo
        /// </summary>
        /// <param name="cuenta"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        [HttpGet("{cuenta}/{fechaInicio}/{fechaFin}")]
        public async Task<IActionResult> Get(string cuenta, DateTime fechaInicio, DateTime fechaFin)
        {
            var cuentaContable = _cuentaService.FindCuentaByNumero(cuenta);
            var movimientoCuentas = _cuentaService.MovimientosDeCuentaYDescendientes(cuentaContable.Id, fechaInicio, fechaFin);
            var movimientos = movimientoCuentas.Select(m => new MovimientoDto
            {
                Id = m.Id,
                Cuenta = m.Cuenta.Numero,
                Fecha = m.Asiento.Fecha,
                Descripcion = m.Asiento.Detalle,
                Importe = _cuentaHelper.ImporteMovimiento(m.Cuenta.Naturaleza, m.TipoDeOperacion, m.Importe),
                TipoDeOperacion = m.TipoDeOperacion
            }).ToList();
            return Ok(movimientos);
        }
    }
}