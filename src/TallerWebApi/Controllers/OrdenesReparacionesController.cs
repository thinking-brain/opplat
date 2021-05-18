using System;
using System.Collections.Generic;
using System.Linq;
using TallerWebApi.Data;
using TallerWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TallerWebApi.Controllers {
    [Route ("taller/[controller]")]
    [ApiController]
    public class OrdenesReparacionesController : Controller {
        private readonly TallerWebApiDbContext context;
        public OrdenesReparacionesController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET TallerWebApi/OrdenesReparaciones
        [HttpGet]
        public ActionResult GetAll () {

            var ordenesReparaciones = context.OrdenesReparaciones.ToList ();
            return Ok (ordenesReparaciones);
        }

        // GET: TallerWebApi/OrdenesReparaciones/Id
        [HttpGet ("{id}", Name = "GetOrdenReparacion")]
        public IActionResult GetbyId (int id) {
            var ordenReparacion = context.OrdenesReparaciones.FirstOrDefault (s => s.Id == id);
            if (ordenReparacion == null) {
                return NotFound ();
            }
            return Ok (ordenReparacion);
        }

        // POST TallerWebApi/OrdenesReparaciones
        [HttpPost]
        public IActionResult POST ([FromBody] OrdenReparacion ordenReparacion) {
           if (ModelState.IsValid) {            
                context.OrdenesReparaciones.Add (ordenReparacion);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetOrdenReparacion", new { id = ordenReparacion.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/OrdenesReparaciones/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] OrdenReparacion ordenReparacion, int id) {
            if (ordenReparacion.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (ordenReparacion).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/OrdenesReparaciones/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var ordenReparacion = context.OrdenesReparaciones.FirstOrDefault (s => s.Id == id);

            if (ordenReparacion.Id != id) {
                return NotFound ();
            }
            context.OrdenesReparaciones.Remove (ordenReparacion);
            context.SaveChanges ();
            return Ok (ordenReparacion);
        }
    }
}