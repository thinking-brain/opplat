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
    public class TalleresController : Controller {
        private readonly TallerWebApiDbContext context;
        public TalleresController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET TallerWebApi/TalleresController
        [HttpGet]
        public ActionResult GetAll () {

            var talleres = context.Talleres.ToList ();
            return Ok (talleres);
        }

        // GET: TallerWebApi/Talleres/Id
        [HttpGet ("{id}", Name = "GetTaller")]
        public IActionResult GetbyId (int id) {
            var taller = context.Talleres.FirstOrDefault (s => s.Id == id);
            if (taller == null) {
                return NotFound ();
            }
            return Ok (taller);
        }

        // POST TallerWebApi/Talleres
        [HttpPost]
        public IActionResult POST ([FromBody] Taller taller) {
             if (ModelState.IsValid) {            
                context.Talleres.Add (taller);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetTaller", new { id = taller.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/Talleres/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Taller taller, int id) {
            if (taller.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (taller).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/Talleres/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var taller = context.Talleres.FirstOrDefault (s => s.Id == id);

            if (taller.Id != id) {
                return NotFound ();
            }
            context.Talleres.Remove (taller);
            context.SaveChanges ();
            return Ok (taller);
        }
    }
}