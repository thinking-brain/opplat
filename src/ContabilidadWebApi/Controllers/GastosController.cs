using System;
using System.Linq;
using System.Threading.Tasks;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Dtos;
using ContabilidadWebApi.Helpers;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class GastosController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;
        private readonly CuentasServices _cuentaService;
        private readonly CuentasHelper _cuentaHelper;

        public GastosController(ContabilidadDbContext context, CuentasServices cuentaService)
        {
            _context = context;
            _cuentaService = cuentaService;
            _cuentaHelper = new CuentasHelper(context);
        }

        /// <summary>
        /// Devuelve listado de operaciones de una cuenta en un periodo de tiempo
        /// </summary>
        /// <param name="codigoDeElemento"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        [HttpGet("{codigoDeElemento}/{fechaInicio}/{fechaFin}")]
        public async Task<IActionResult> Get(string codigoDeElemento, DateTime fechaInicio, DateTime fechaFin)
        {
            var elemento = await _context.Set<ElementoDeGasto>()
                .SingleOrDefaultAsync(e => e.Codigo == codigoDeElemento);
            var gastos = await _context.Set<RegistroDeGasto>()
                .Include(r => r.Asiento)
                .Include(r => r.SubElemento.Elemento)
                .Where(r => r.Asiento.Fecha.Date >= fechaInicio
                    && r.Asiento.Fecha.Date <= fechaFin
                    && r.SubElemento.Elemento.Codigo == codigoDeElemento)
                .Select(r => new
                {
                    Id = r.Id,
                    Fecha = r.Asiento.Fecha,
                    Elemento = $"{r.SubElemento.Elemento.Codigo} - {r.SubElemento.Elemento.Descripcion}",
                    SubElemento = $"{r.SubElemento.Codigo} - {r.SubElemento.Descripcion}",
                    Descripcion = r.Asiento.Detalle,
                    Importe = r.Importe
                })
                .ToListAsync();
            return Ok(gastos);
        }
    }
}