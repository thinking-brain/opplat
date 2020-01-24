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
    public class ContratosController : Controller {
        private readonly RhWebApiDbContext context;
        public ContratosController (RhWebApiDbContext context) {
            this.context = context;
        }

        // GET recursos_humanos/Contrato
        [HttpGet]
        public IEnumerable<Contrato> GetAll () {
            return context.Contrato.ToList ();
        }

        // GET: recursos_humanos/Contrato/Id
        [HttpGet ("{id}", Name = "GetContrato")]
        public IActionResult GetbyId (int id) {
            var contrato = context.Contrato.FirstOrDefault (s => s.Id == id);

            if (contrato == null) {
                return NotFound ();
            }
            return Ok (contrato);

        }

        // POST recursos_humanos/Contrato
        [HttpPost]
        public IActionResult POST ([FromBody] Contrato contrato) {
            if (ModelState.IsValid) {
                context.Contrato.Add (contrato);
                context.SaveChanges ();
                return new CreatedAtRouteResult ("GetContrato", new { id = contrato.Id });
            }
            return BadRequest (ModelState);
        }

        // PUT recursos_humanos/contrato/id
        [HttpPut ("{id}")]
        public IActionResult PUT ([FromBody] Contrato contrato, int id) {
            if (contrato.Id != id) {
                return BadRequest (ModelState);

            }
            context.Entry (contrato).State = EntityState.Modified;
            context.SaveChanges ();
            return Ok ();
        }

        // DELETE recursos_humanos/contrato/id
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            var contrato = context.Contrato.FirstOrDefault (s => s.Id == id);

            if (contrato.Id != id) {
                return NotFound ();
            }
            context.Contrato.Remove (contrato);
            context.SaveChanges ();
            return Ok (contrato);
        }
    }
}