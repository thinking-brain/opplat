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
    public class NomPeriodopagoController : ControllerBase
    {
        
        private readonly VersatDbContext _context;

        public NomPeriodopagoController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Periodos de Pago
        /// </summary>
        /// <returns></returns>
        // GET: api/NomPeriodopagoOne
        [HttpGet]
        public IEnumerable<NomPeriodopago> GetNomPeriodopago()
        {
            return _context.NomPeriodopago;
        }

        /// <summary>
        /// Devuelve un Periodo de Pago segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/GetNomPeriodopago/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNomPeriodopago([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nomPeriodopago = await _context.NomPeriodopago.FindAsync(id);

            if (nomPeriodopago == null)
            {
                return NotFound();
            }

            return Ok(nomPeriodopago);
        }
       
        /// <summary>
        /// Chequea que Exista el Periodo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool NomPeriodopagoExists(int id)
        {
            return _context.NomPeriodopago.Any(e => e.Idperiodopago == id);
        }
    }
}