using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.Data;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public AreasController(ApiDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un listado con todas las areas
        /// </summary>
        /// <returns></returns>
        // GET: api/Areas
        [HttpGet]
        public IEnumerable<Area> GetArea()
        {
            return _context.Area;
        }

        /// <summary>
        /// Devuelve un area segun ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Areas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArea([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var area = await _context.Area.FindAsync(id);

            if (area == null)
            {
                return NotFound();
            }

            return Ok(area);
        }

        /// <summary>
        /// Editar Area
        /// </summary>
        /// <param name="id"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        // PUT: api/Areas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea([FromRoute] int id, [FromBody] Area area)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != area.Id)
            {
                return BadRequest();
            }

            _context.Entry(area).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        /// <summary>
        /// Crear Area
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        // POST: api/Areas
        [HttpPost]
        public async Task<IActionResult> PostArea([FromBody] Area area)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Area.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArea", new { id = area.Id }, area);
        }
        /// <summary>
        /// Eliminar Area
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Areas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var area = await _context.Area.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            _context.Area.Remove(area);
            await _context.SaveChangesAsync();

            return Ok(area);
        }
        /// <summary>
        /// Comprobar si existe el Area
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool AreaExists(int id)
        {
            return _context.Area.Any(e => e.Id == id);
        }
    }
}