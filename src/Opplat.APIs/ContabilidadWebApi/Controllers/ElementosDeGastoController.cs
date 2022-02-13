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
    public class ElementosDeGastoController : ControllerBase
    {
        private readonly ContabilidadDbContext _context;
        private readonly CuentasServices _cuentaService;
        private readonly CuentasHelper _cuentaHelper;

        public ElementosDeGastoController(ContabilidadDbContext context, CuentasServices cuentaService)
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
            var elementos = await _context.Set<ElementoDeGasto>().Select(e => new
            {
                e.Id,
                e.Codigo,
                e.Descripcion,
                e.Activo
            }).ToListAsync();
            return Ok(elementos);
        }

        /// <summary>
        /// Devuelve el elemento de gasto por el id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int:max(3)}")]
        public async Task<IActionResult> Get(int id)
        {
            var elemento = await _context.Set<ElementoDeGasto>().SingleOrDefaultAsync(e => e.Id == id);
            if (elemento == null)
            {
                return NotFound();
            }
            return Ok(elemento);
        }

        /// <summary>
        /// Devuelve el elemento de gasto por el codigo
        /// </summary>
        /// <param name="codigo:alpha"></param>
        /// <returns></returns>
        [HttpGet("{codigo}")]
        public async Task<IActionResult> Get(string codigo)
        {
            var elemento = await _context.Set<ElementoDeGasto>().SingleOrDefaultAsync(e => e.Codigo == codigo);
            if (elemento == null)
            {
                return NotFound();
            }
            return Ok(elemento);
        }

        /// <summary>
        /// Agrega un elemento de gasto
        /// </summary>
        /// <param name="elemento">Datos de la nueva cuenta</param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Post([FromBody] ElementoDeGasto elemento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _context.Add(elemento);
                _context.SaveChanges();
                return Ok(elemento);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Modifica un elemento de gasto
        /// </summary>
        /// <param name="id">Id de elemento de gasto</param>
        /// <param name="elemento">Datos del elemento de gasto</param>
        /// <returns></returns>
        [HttpPut()]
        public async Task<IActionResult> Put(int id, [FromBody] ElementoDeGasto elemento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_context.Set<ElementoDeGasto>().Any(c => c.Id == id))
            {
                return NotFound($"No se encuentra el elemento de gasto con id {id}");
            }
            try
            {
                _context.Update(elemento);
                _context.SaveChanges();
                return Ok(elemento);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina un elemento de gasto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var elemento = await _context.Set<ElementoDeGasto>().FirstOrDefaultAsync(c => c.Id == id);
            if (elemento == null)
            {
                return NotFound($"No se encuentra el elemento de gasto con id {id}");
            }
            try
            {
                _context.Remove(elemento);
                await _context.SaveChangesAsync();
                return Ok("Elemento de gasto eliminado correctamente.");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}