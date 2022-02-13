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
    public class SubElementosDeGastoController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;
        private readonly CuentasServices _cuentaService;
        private readonly CuentasHelper _cuentaHelper;

        public SubElementosDeGastoController(ContabilidadDbContext context, CuentasServices cuentaService)
        {
            _context = context;
            _cuentaService = cuentaService;
            _cuentaHelper = new CuentasHelper(context);
        }

        /// <summary>
        /// Devuelve todos los elementos de gasto
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var elementos = await _context.Set<SubElementoDeGasto>().Select(e => new
            {
                e.Id,
                e.Codigo,
                e.Descripcion,
                e.Activo
            }).ToListAsync();
            return Ok(elementos);
        }

        /// <summary>
        /// Devuelve el subelemento de gasto por el id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int:max(3)}")]
        public async Task<IActionResult> Get(int id)
        {
            var subelemento = await _context.Set<SubElementoDeGasto>().SingleOrDefaultAsync(e => e.Id == id);
            if (subelemento == null)
            {
                return NotFound();
            }
            return Ok(subelemento);
        }

        /// <summary>
        /// Devuelve el subelemento de gasto por el codigo
        /// </summary>
        /// <param name="codigo:alpha"></param>
        /// <returns></returns>
        [HttpGet("{codigo}")]
        public async Task<IActionResult> Get(string codigo)
        {
            var subelemento = await _context.Set<SubElementoDeGasto>().SingleOrDefaultAsync(e => e.Codigo == codigo);
            if (subelemento == null)
            {
                return NotFound();
            }
            return Ok(subelemento);
        }

        /// <summary>
        /// Agrega un subelemento de gasto
        /// </summary>
        /// <param name="subelemento">Datos de la nueva cuenta</param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Post([FromBody] SubElementoDeGasto subelemento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _context.Add(subelemento);
                _context.SaveChanges();
                return Ok(subelemento);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Modifica un subelemento de gasto
        /// </summary>
        /// <param name="id">Id del subelemento</param>
        /// <param name="subelemento">Datos del subelemento</param>
        /// <returns></returns>
        [HttpPut()]
        public async Task<IActionResult> Put(int id, [FromBody] SubElementoDeGasto subelemento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Set<SubElementoDeGasto>().Any(c => c.Id == id))
            {
                return NotFound($"No se encuentra el subelemento de gasto con id {id}");
            }
            try
            {
                _context.Update(subelemento);
                _context.SaveChanges();
                return Ok(subelemento);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina un subelemento de gasto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subelemento = await _context.Set<SubElementoDeGasto>().FirstOrDefaultAsync(c => c.Id == id);
            if (subelemento == null)
            {
                return NotFound($"No se encuentra el subelemento de gasto con id {id}");
            }
            try
            {
                _context.Remove(subelemento);
                await _context.SaveChangesAsync();
                return Ok("Subelemento de gasto eliminado correctamente.");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}