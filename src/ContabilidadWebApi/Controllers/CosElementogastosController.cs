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
    public class CosElementogastosController : ControllerBase
    {
        private readonly VersatDbContext _context;

        public CosElementogastosController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Elmentos de Gastos
        /// </summary>
        /// <returns></returns>
        // GET: api/CosElementogastos
        [HttpGet]
        public IEnumerable<CosElementogasto> GetCosElementogasto()
        {
            return _context.CosElementogasto;
        }
        /// <summary>
        /// Devuelve un Elemento de Gasto segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/CosElementogastos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCosElementogasto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cosElementogasto = await _context.CosElementogasto.FindAsync(id);

            if (cosElementogasto == null)
            {
                return NotFound();
            }

            return Ok(cosElementogasto);
        }
      

        /// <summary>
        /// Chequea que Exista el Elemento de Gasto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CosElementogastoExists(int id)
        {
            return _context.CosElementogasto.Any(e => e.Idelementogasto == id);
        }
    }
}