using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.VersatModels;
using ContabilidadWebApi.ViewModels;

namespace ContabilidadWebApi.Controllers
{
    [Route("contabilidad/[controller]")]
    [ApiController]
    public class NomDocumentoDetallePagoController : ControllerBase
    {
        
        private readonly VersatDbContext _context;

        public NomDocumentoDetallePagoController(VersatDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve un Listado de Periodos de Pago
        /// </summary>
        /// <returns></returns>
        // GET: api/NomDocumentoDetallePago
        [HttpGet]
        public List<NomDocumentoDetallePagoVM> GetNomDocumento()
        {
            var nomDetPago = new List<NomDocumentoDetallePagoVM>();

            var data = _context.NomDocumentoDetallePago.Include(s=>s.IddocumentoNavigation).Join(_context.NomDocumento.Include(s=>s.IdperiodopagoNavigation), 
                detalleDoc => detalleDoc.Iddocumento,
                docum => docum.Iddocumento, (detalleDoc, docum) => new NomDocumentoDetallePagoVM
                {
                    Iddocumento = detalleDoc.Iddocumento,
                    NCobrar = detalleDoc.NCobrar,
                    Idcuenta = detalleDoc.Idcuenta,
                    IdPeriodo = docum.Idperiodopago,
                    Año = docum.IdperiodopagoNavigation.PeriodoIni.Value.Year,
                    Mes = docum.IdperiodopagoNavigation.PeriodoFin.Value.Month,

                });
            return data.ToList();
        }

        /// <summary>
        /// Devuelve un Documento Detalle de Pago segun su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/GetNomDocumentoDetallePago/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNomDocumentoDetallePago([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nomDocumentoDetalle = await _context.NomDocumentoDetallePago.FindAsync(id);

            if (nomDocumentoDetalle == null)
            {
                return NotFound();
            }

            return Ok(nomDocumentoDetalle);
        }
       
        /// <summary>
        /// Chequea que Exista el Periodo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool NomDocumentoDetalleExists(int id)
        {
            return _context.NomDocumentoDetallePago.Any(e => e.Iddocumento == id);
        }
    }
}