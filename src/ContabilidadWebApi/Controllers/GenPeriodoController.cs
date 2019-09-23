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
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class GenPeriodoController : ControllerBase
    {
        
        private readonly VersatDbContext _context;

        public GenPeriodoController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Periodos
        /// </summary>
        /// <returns></returns>
        // GET: api/GenPeriodo
        [HttpGet]
        public IEnumerable<GenPeriodo> GetPeriodo()
        {
            return _context.GenPeriodo;
        }

        /// <summary>
        /// Devuelve un Periodo segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/GetNomPeriodopago/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenPeriodo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var genPeriodo = await _context.GenPeriodo.FindAsync(id);

            if (genPeriodo == null)
            {
                return NotFound();
            }

            return Ok(genPeriodo);
        }
       
        /// <summary>
        /// Chequea que Exista el Periodo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool GenPeriodoExists(int id)
        {
            return _context.GenPeriodo.Any(e => e.Idperiodo == id);
        }
    }
}