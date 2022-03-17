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
    public class PresupuestosController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public PresupuestosController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET TallerWebApi/Presupuestos
        [HttpGet]
        public ActionResult GetAll () {
            var presupuestos = context.Presupuestos.ToList ();
            return Ok (presupuestos);
        }

        // GET: TallerWebApi/Presupuestos/Id
        [HttpGet ("{id}", Name = "GetPresupuesto")]
        public IActionResult GetbyId (int id) {
            var presupuesto = context.Presupuestos.FirstOrDefault (s => s.Id == id);
            if (presupuesto == null) {
                return NotFound ();
            }
            return Ok (presupuesto);
        }

        // POST TallerWebApi/Presupuestos
        [HttpPost]
        public IActionResult POST ([FromBody] Presupuesto presupuesto) {
             if (ModelState.IsValid) {            
                context.Presupuestos.Add (presupuesto);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetPresupuesto", new { id = presupuesto.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Presupuestos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Presupuesto presupuesto, int id) {
            if (presupuesto.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (presupuesto).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Presupuestos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var presupuesto = context.Presupuestos.FirstOrDefault (s => s.Id == id);

            if (presupuesto.Id != id) {
                return NotFound ();
            }
            context.Presupuestos.Remove (presupuesto);
            context.SaveChanges ();
            return Ok (presupuesto);
        }
    }
}