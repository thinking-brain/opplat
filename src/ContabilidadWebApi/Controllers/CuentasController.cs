using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.VersatModels;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Helpers;
using ContabilidadWebApi.Dtos;
using ContabilidadWebApi.Services;
using ContabilidadWebApi.Models;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;

        public CuentasController(ContabilidadDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve el clasificador de cuentas
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ctaHelper = new CuentasHelper(_context);
            return Ok(ctaHelper.Cuentas());
        }

        /// <summary>
        /// Devuelve una Cuenta segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCuenta([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ctaHelper = new CuentasHelper(_context);

            var cuentas = ctaHelper.Cuentas();
            var cta = cuentas.SingleOrDefault(c => c.Id == id);

            if (cta == null)
            {
                return NotFound();
            }
            return Ok(cta);
        }

        /// <summary>
        /// Agrega una cuenta nueva
        /// </summary>
        /// <param name="nuevaCuenta">Datos de la nueva cuenta</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] NuevaCuentaDto nuevaCuenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var servicio = new CuentasServices(_context);
                if (servicio.FindCuentaByNumero(nuevaCuenta.Numero) != null)
                {
                    return BadRequest("Ya esta cuenta esta creada");
                }
                var cuenta = servicio.CrearCuenta(nuevaCuenta.Numero, nuevaCuenta.Nombre, nuevaCuenta.Naturaleza);
                return Ok(cuenta);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Modifica una cuenta
        /// </summary>
        /// <param name="id">Id de la cuenta</param>
        /// <param name="cuenta">Datos de la cuenta</param>
        /// <returns></returns>
        [HttpPut()]
        public async Task<IActionResult> Put(int id, [FromBody] NuevaCuentaDto cuenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Set<Cuenta>().Any(c => c.Id == id))
            {
                return NotFound($"No se encuentra la cuenta con id {id}");
            }
            try
            {
                var servicio = new CuentasServices(_context);
                var cta = servicio.EditarCuenta(id, cuenta.Numero, cuenta.Nombre, cuenta.Naturaleza);
                return Ok(cta);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Modifica una cuenta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cuenta = await _context.Set<Cuenta>().FirstOrDefaultAsync(c => c.Id == id);
            if (cuenta == null)
            {
                return NotFound($"No se encuentra la cuenta con id {id}");
            }
            var servicio = new CuentasServices(_context);
            var movimientos = servicio.GetMovimientosDeCuenta(id);
            if (movimientos.Any())
            {
                return BadRequest("No se puede eliminar una cuenta con movimientos.");
            }
            try
            {
                _context.Remove(cuenta);
                await _context.SaveChangesAsync();
                return Ok("Cuenta eliminada correctamente.");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}