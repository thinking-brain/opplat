using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers {
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class TiempoVenOfertasController : ControllerBase {
        private readonly ContratacionDbContext context;
        public TiempoVenOfertasController (ContratacionDbContext context) {
            this.context = context;
        }

        // GET contratacion/TiempoVenOfertas
        [HttpGet]
        public IEnumerable<TiempoVenOferta> GetAll () {
            return context.TiempoVenOfertas.ToList ();
        }

        // GET: contratacion/TiempoVenOfertas/Id
        [HttpGet ("{id}", Name = "GetTiempoVenOferta")]
        public IActionResult GetbyId (int id) {
            var tiempoVenOferta = context.TiempoVenOfertas.FirstOrDefault (s => s.Id == id);

            if (tiempoVenOferta == null) {
                return NotFound ();
            }
            return Ok (tiempoVenOferta);
        }

        // POST contratacion/TiempoVenOfertas
        [HttpPost]
        public IActionResult POST ([FromBody] TiempoVenOferta tiempoVenOferta) {
            if (ModelState.IsValid) {
                var t = context.TiempoVenOfertas.ToList ();
                if (t != null) {
                    foreach (var item in t) {
                        context.TiempoVenOfertas.Remove (item);
                    }
                }
                context.TiempoVenOfertas.Add (tiempoVenOferta);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTiempoVenOferta", new { id = tiempoVenOferta.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT contratacion/TiempoVenOfertas/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] TiempoVenOferta tiempoVenOferta, int id) {
            if (tiempoVenOferta.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (tiempoVenOferta).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE contratacion/TiempoVenOfertas/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var tiempoVenOferta = context.TiempoVenOfertas.FirstOrDefault (s => s.Id == id);

            if (tiempoVenOferta.Id != id) {
                return NotFound ();
            }
            context.TiempoVenOfertas.Remove (tiempoVenOferta);
            context.SaveChanges ();
            return Ok (tiempoVenOferta);
        }
    }
}