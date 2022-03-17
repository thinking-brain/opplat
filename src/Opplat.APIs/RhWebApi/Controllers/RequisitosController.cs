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
    public class RequisitosController : ControllerBase {
        private readonly RhWebApiDbContext context;
        public RequisitosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/Requisitos
        [HttpGet]
        public IEnumerable<Requisitos> GetAll () {
            return context.Requisitos.ToList ();
        }

        // GET: recursos_humanos/Requisitos/Id
        [HttpGet ("{id}", Name = "GetRequisitos")]
        public IActionResult GetbyId (int id) {
            var requisitos = context.Requisitos.FirstOrDefault (s => s.Id == id);

            if (requisitos == null) {
                return NotFound ();
            }
            return Ok (requisitos);

        }

        // POST recursos_humanos/Requisitos
        [HttpPost]
        public IActionResult POST ([FromBody] Requisitos requisitos) {
            if (ModelState.IsValid) {
                context.Requisitos.Add (requisitos);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetRequisitos", new { id = requisitos.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/Requisitos/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Requisitos requisitos, int id) {
            if (requisitos.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (requisitos).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/Requisitos/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var requisitos = context.Requisitos.FirstOrDefault (s => s.Id == id);

            if (requisitos.Id != id) {
                return NotFound ();
            }
            context.Requisitos.Remove (requisitos);
            context.SaveChanges ();
            return Ok (requisitos);
        }
    }
}