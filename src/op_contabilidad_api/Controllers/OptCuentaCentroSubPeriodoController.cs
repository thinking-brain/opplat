using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using op_contabilidad_api.VersatModels;

namespace op_contabilidad_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptCuentaCentroSubPeriodoController : ControllerBase
    {
        
        private readonly VersatDbContext _context;

        public OptCuentaCentroSubPeriodoController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Periodos
        /// </summary>
        /// <returns></returns>
        // GET: api/OptCuentaCentroSubPeriodo
        [HttpGet]
        public IEnumerable<OptCuentaCentroSubPeriodo> GetOptCuentaCentroSubPeriodo()
        {
            return _context.OptCuentaCentroSubPeriodo;
        }

        /// <summary>
        /// Devuelve un Registro de OptCuentaCentroSubPeriodo segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/OptCuentaCentroSubPeriodo/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOptCuentaCentroSubPeriodo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var opt = await _context.OptCuentaCentroSubPeriodo.FindAsync(id);

            if (opt == null)
            {
                return NotFound();
            }

            return Ok(opt);
        }
       
        /// <summary>
        /// Chequea que Exista el Registro de OptCuentaCentroSubPeriodo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool OptCuentaCentroSubPeriodoExists(int id)
        {
            return _context.OptCuentaCentroSubPeriodo.Any(e => e.Idperiodo == id);
        }
    }
}