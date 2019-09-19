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
    public class NomDocumentoController : ControllerBase
    {
        
        private readonly VersatDbContext _context;

        public NomDocumentoController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Periodos de Pago
        /// </summary>
        /// <returns></returns>
        // GET: api/NomDocumentoDetallePago
        [HttpGet]
        public IEnumerable<NomDocumento> GetNomDocumento()
        {
            return _context.NomDocumento;
        }

        /// <summary>
        /// Devuelve un Documento Detalle de Pago segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/GetNomDocumento/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNomDocumento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nomDocumento = await _context.NomDocumento.FindAsync(id);

            if (nomDocumento == null)
            {
                return NotFound();
            }

            return Ok(nomDocumento);
        }
       
        /// <summary>
        /// Chequea que Exista el Periodo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool NomDocumentoExists(int id)
        {
            return _context.NomDocumento.Any(e => e.Iddocumento == id);
        }
    }
}