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
    public class CosSubelementogastosController : ControllerBase
    {
        private readonly VersatDbContext _context;

        public CosSubelementogastosController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de SubElementos de Gasto
        /// </summary>
        /// <returns></returns>
        // GET: api/CosSubelementogastos
        [HttpGet]
        public IEnumerable<CosSubelementogasto> GetCosSubelementogasto()
        {
            return _context.CosSubelementogasto;
        }
        /// <summary>
        /// Devuelve un SubElemento de Gasto Segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CosSubelementogastos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCosSubelementogasto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cosSubelementogasto = await _context.CosSubelementogasto.FindAsync(id);

            if (cosSubelementogasto == null)
            {
                return NotFound();
            }

            return Ok(cosSubelementogasto);
        }

        
        /// <summary>
        /// Chequea que Exista el SubElemento de Gasto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CosSubelementogastoExists(int id)
        {
            return _context.CosSubelementogasto.Any(e => e.Idsubelemento == id);
        }
    }
}