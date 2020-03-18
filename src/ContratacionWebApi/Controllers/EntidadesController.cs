using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContratacionWebApi.Controllers
{
    [Route ("contratacion/[controller]")]
    [ApiController]
    public class EntidadesController : Controller {
        private readonly ContratacionDbContext context;
        public EntidadesController (ContratacionDbContext context) {
            this.context = context;
        }

        // GET entidades/Entidades
         [HttpGet]
        public IEnumerable<Entidad> GetAll()
        {                            
            return context.Entidades.ToList();            
        }       

        // GET: entidades/Entidades/Id
        [HttpGet ("{id}", Name = "GetEntidad")]
        public IActionResult GetbyId (int id) {
            var entidad = context.Entidades.FirstOrDefault (s => s.Id == id);

            if (entidad == null) {
                return NotFound ();
            }
            return Ok (entidad);
        }

        // POST entidades/Entidades
        [HttpPost]
        public IActionResult POST ([FromBody] Entidad entidad) {
            if (ModelState.IsValid) {             
                context.Entidades.Add (entidad);
                context.SaveChanges ();             
                return new CreatedAtRouteResult ("GetEntidad", new { id = entidad.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT entidades/entidad/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Entidad entidad, int id) {
            if (entidad.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (entidad).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE entidades/entidad/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var entidad = context.Entidades.FirstOrDefault (s => s.Id == id);

            if (entidad.Id != id) {
                return NotFound ();
            }
            context.Entidades.Remove (entidad);
            context.SaveChanges ();
            return Ok (entidad);
        }
    }
}