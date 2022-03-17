using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhWebApi.Models;
using RhWebApi.Data;


namespace RhWebApi.Controllers {
    [Route ("recursos_humanos/[controller]")]
    [ApiController]
    public class ContratoTrabsController : ControllerBase {
        private readonly RhWebApiDbContext context;
        public ContratoTrabsController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/ContratoTrab
        [HttpGet]
        public IEnumerable<ContratoTrab> GetAll () {
            return context.ContratoTrab.ToList ();
        }

        // GET: recursos_humanos/ContratoTrab/Id
        [HttpGet ("{id}", Name = "GetContratoTrab")]
        public IActionResult GetbyId (int id) {
            var ContratoTrab = context.ContratoTrab.FirstOrDefault (s => s.Id == id);

            if (ContratoTrab == null) {
                return NotFound ();
            }
            return Ok (ContratoTrab);

        }

        // POST recursos_humanos/ContratoTrab
        [HttpPost]
        public IActionResult POST ([FromBody] ContratoTrab ContratoTrab) {
            if (ModelState.IsValid) {
                context.ContratoTrab.Add (ContratoTrab);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetContratoTrab", new { id = ContratoTrab.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/ContratoTrab/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] ContratoTrab ContratoTrab, int id) {
            if (ContratoTrab.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (ContratoTrab).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/ContratoTrab/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var ContratoTrab = context.ContratoTrab.FirstOrDefault (s => s.Id == id);

            if (ContratoTrab.Id != id) {
                return NotFound ();
            }
            context.ContratoTrab.Remove (ContratoTrab);
            context.SaveChanges ();
            return Ok (ContratoTrab);
        }
    }
}