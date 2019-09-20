using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.VersatModels;

namespace ContabilidadWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosCentrosController : ControllerBase
    {
        private readonly VersatDbContext _context;

        public CosCentrosController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Centros de Costos
        /// </summary>
        /// <returns></returns>
        // GET: api/CosCentros
        [HttpGet]
        public IEnumerable<CosCentro> GetCosCentro()
        {
            return _context.CosCentro;
        }
        /// <summary>
        /// Devuelve un Centro de Costo segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CosCentros/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCosCentro([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cosCentro = await _context.CosCentro.FindAsync(id);

            if (cosCentro == null)
            {
                return NotFound();
            }

            return Ok(cosCentro);
        }

        /// <summary>
        /// Chequea que Exista el Centro de Costo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CosCentroExists(int id)
        {
            return _context.CosCentro.Any(e => e.Idcentro == id);
        }
    }
}