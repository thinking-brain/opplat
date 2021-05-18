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
    public class RepuestosController : Controller {
        private readonly TallerWebApiDbContext context;
        public RepuestosController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET TallerWebApi/Repuestos
        [HttpGet]
        public ActionResult GetAll () {
            var repuestos = context.Repuestos.ToList ();
            return Ok (repuestos);
        }

        // GET: TallerWebApi/Repuestos/Id
        [HttpGet ("{id}", Name = "GetRepuesto")]
        public IActionResult GetbyId (int id) {
            var repuesto = context.Repuestos.FirstOrDefault (s => s.Id == id);
            if (repuesto == null) {
                return NotFound ();
            }
            return Ok (repuesto);
        }

        // POST TallerWebApi/Repuestos
        [HttpPost]
        public IActionResult POST ([FromBody] Repuesto repuesto) {
             if (ModelState.IsValid) {            
                context.Repuestos.Add (repuesto);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetRepuesto", new { id = repuesto.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Repuestos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Repuesto repuesto, int id) {
            if (repuesto.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (repuesto).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Repuestos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var repuesto = context.Repuestos.FirstOrDefault (s => s.Id == id);

            if (repuesto.Id != id) {
                return NotFound ();
            }
            context.Repuestos.Remove (repuesto);
            context.SaveChanges ();
            return Ok (repuesto);
        }
    }
}