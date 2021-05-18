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
    public class TecnicosController : Controller {
        private readonly TallerWebApiDbContext context;
        public TecnicosController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET TallerWebApi/Tecnicos
        [HttpGet]
        public ActionResult GetAll () {

            var tecnicos = context.Tecnicos.ToList ();
            return Ok (tecnicos);
        }

        // GET: TallerWebApi/Tecnicos/Id
        [HttpGet ("{id}", Name = "GetTecnico")]
        public IActionResult GetbyId (int id) {
            var tecnico = context.Tecnicos.FirstOrDefault (s => s.Id == id);
            if (tecnico == null) {
                return NotFound ();
            }
            return Ok (tecnico);
        }

        // POST TallerWebApi/Tecnicos
        [HttpPost]
        public IActionResult POST ([FromBody] Tecnico tecnico) {
            if (ModelState.IsValid) {            
                context.Tecnicos.Add (tecnico);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTecnico", new { id = tecnico.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Tecnicos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Tecnico tecnico, int id) {
            if (tecnico.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (tecnico).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Tecnicos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var tecnico = context.Tecnicos.FirstOrDefault (s => s.Id == id);

            if (tecnico.Id != id) {
                return NotFound ();
            }
            context.Tecnicos.Remove (tecnico);
            context.SaveChanges ();
            return Ok (tecnico);
        }
    }
}