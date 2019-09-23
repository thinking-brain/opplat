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
    public class ConCuentasNatController : ControllerBase
    {
        private readonly VersatDbContext _context;

        public ConCuentasNatController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Cuentas con su Naturaleza
        /// </summary>
        /// <returns></returns>
        // GET: api/ConCuentas
        [HttpGet]
        public IEnumerable<ConCuentanat> GetConCuenta()
        {
            return _context.ConCuentanat;
        }
        /// <summary>
        /// Devuelve una Cuenta segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/GetConCuenta/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConCuenta([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conCuenta = await _context.ConCuentanat.FindAsync(id);

            if (conCuenta == null)
            {
                return NotFound();
            }

            return Ok(conCuenta);
        }
       
        /// <summary>
        /// Chequea que Exista la cuenta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ConCuentaExists(int id)
        {
            return _context.ConCuentanat.Any(e => e.Idcuenta == id);
        }
    }
}