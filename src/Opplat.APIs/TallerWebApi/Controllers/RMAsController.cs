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
    public class RMAsController : ControllerBase {
        private readonly TallerWebApiDbContext context;
        public RMAsController (TallerWebApiDbContext context) {
            this.context = context;
        }
        // GET TallerWebApi/RMAs
        [HttpGet]
        public ActionResult GetAll () {
            var rmas = context.RMAs.ToList ();
            return Ok (rmas);
        }

        // GET: TallerWebApi/RMAs/Id
        [HttpGet ("{id}", Name = "GetRMA")]
        public IActionResult GetbyId (int id) {
            var rma = context.RMAs.FirstOrDefault (s => s.Id == id);
            if (rma == null) {
                return NotFound ();
            }
            return Ok (rma);
        }

        // POST TallerWebApi/RMAs
        [HttpPost]
        public IActionResult POST ([FromBody] RMA rma) {
             if (ModelState.IsValid) {            
                context.RMAs.Add (rma);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetRMA", new { id = rma.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT TallerWebApi/RMAs/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] RMA rma, int id) {
            if (rma.Id != id) {
                return BadRequest (ModelState);
            }
            context.Entry (rma).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE TallerWebApi/RMAs/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var rma = context.RMAs.FirstOrDefault (s => s.Id == id);

            if (rma.Id != id) {
                return NotFound ();
            }
            context.RMAs.Remove (rma);
            context.SaveChanges ();
            return Ok (rma);
        }
    }
}