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
    [Route("api/[controller]")]
    [ApiController]
    public class GrupoSubelementosController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public GrupoSubelementosController(ApiDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Grupo de SubElementos
        /// </summary>
        /// <returns></returns>
        // GET: api/GrupoSubelementos
        [HttpGet]
        public IEnumerable<GrupoSubelemento> GetGrupoSubelemento()
        {
            return _context.GrupoSubelemento;
        }
        /// <summary>
        /// Devuelve un Grupo de SubElementos segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/GrupoSubelementos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrupoSubelemento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grupoSubelemento = await _context.GrupoSubelemento.FindAsync(id);

            if (grupoSubelemento == null)
            {
                return NotFound();
            }

            return Ok(grupoSubelemento);
        }
        /// <summary>
        /// Editar un Grupo de SubElementos
        /// </summary>
        /// <param name="id"></param>
        /// <param name="grupoSubelemento"></param>
        /// <returns></returns>
        // PUT: api/GrupoSubelementos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGrupoSubelemento([FromRoute] int id, [FromBody] GrupoSubelemento grupoSubelemento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != grupoSubelemento.Id)
            {
                return BadRequest();
            }

            _context.Entry(grupoSubelemento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrupoSubelementoExists(id))
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
        /// Crear un Grupo de SubElementos
        /// </summary>
        /// <param name="grupoSubelemento"></param>
        /// <returns></returns>
        // POST: api/GrupoSubelementos
        [HttpPost]
        public async Task<IActionResult> PostGrupoSubelemento([FromBody] GrupoSubelemento grupoSubelemento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.GrupoSubelemento.Add(grupoSubelemento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGrupoSubelemento", new { id = grupoSubelemento.Id }, grupoSubelemento);
        }
        /// <summary>
        /// Eliminar un Grupo de SubElementos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/GrupoSubelementos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrupoSubelemento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grupoSubelemento = await _context.GrupoSubelemento.FindAsync(id);
            if (grupoSubelemento == null)
            {
                return NotFound();
            }

            _context.GrupoSubelemento.Remove(grupoSubelemento);
            await _context.SaveChangesAsync();

            return Ok(grupoSubelemento);
        }

        private bool GrupoSubelementoExists(int id)
        {
            return _context.GrupoSubelemento.Any(e => e.Id == id);
        }
    }
}